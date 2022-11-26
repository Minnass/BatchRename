# BatchRename

## Small project through learning Window Programming

# All detail:

## Technical details:
- Design patterns: Singleton, Factory, Abstract factory, prototype
- Plugin architecture
- Delegate & event
## Core requirements:
1. Dynamically load all renaming rules from external DLL files
2. Select all files and folders you want to rename
3. Create a set of rules for renaming the files
 - Each rule can be added from a menu
 - Each rule's parameters can be edited  
4. Apply the set of rules in numerical order to each file, make them have a new name
5. Save this set of rules into presets for quickly loading later if you need to reuse
## Renaming rules:
1. Change the extension to another extension (no conversion, force renaming extension)
2. Add counter to the end of the file
- Could specify the start value, steps, number of digits (Could have padding like 01, 02, 03...10....99)
3. Remove all space from the beginning and the ending of the filename
4. Replace certain characters into one character like replacing "-" ad "_" into space " "
- Could be the other way like replace all space " " into dot "."
5. Adding a prefix to all the files
6. Adding a suffix to all the files
7. Convert all characters to lowercase, remove all spaces
8. Convert filename to PascalCase.
## Improvements
1. Drag & Drop a file to add to the list
2. Storing parameters for renaming using XML file / JSON / excel / database
3. Adding recursively: just specify a folder only, the application will automatically scan and add all the files inside
4. Handling duplication
5. Last time state: When exiting the application, auto-save and load the 
- The current size of the screen
- Current position of the screen
- Last chosen preset
6. Autosave & load the current working condition to prevent sudden power loss
- The current file list
- The current set of renaming rules, together with the parameters
7. Using regular expressions
8. Checking exceptions when editing rules: like characters that cannot be in the file name, the maximum length of the filename cannot exceed 255 characters
9. Save and load your work into a project.
10. Let the user see the preview of the result
11. Create a copy of all the files and move them to a selected folder rather than perform the renaming on the original file
12. Anything that you think is suitable
