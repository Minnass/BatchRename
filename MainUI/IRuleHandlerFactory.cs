using BasicCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace MainUI
{
    internal class IRuleHandlerFactory
    {
        private Dictionary<string, IRuleHandler> _prototypes = new Dictionary<string, IRuleHandler>();
        public void registerIRuleHandler(IRuleHandler newRule)
        {
            _prototypes.Add(newRule.getRuleType(), newRule);
        }
        private static IRuleHandlerFactory? _instance;
        public static IRuleHandlerFactory getInstance()
        {
            if (_instance == null)
            {
                _instance = new IRuleHandlerFactory();
            }
            return _instance;
        }
        private IRuleHandlerFactory()
        {
            //do nothing heare
        }
        public IRuleHandler? parse(string ruleName)
        {
            
            if(_prototypes.ContainsKey(ruleName))
            {
                IRuleHandler rule = _prototypes[ruleName];
                return (IRuleHandler)rule.Clone();
            }
            else

            {
                return null;
            }
           

        }
        public int totalIruleHandler()
        {
            return _prototypes.Count;
        }

        public BindingList<IRuleHandler> getTotalCurrentRules()
        {
           BindingList<IRuleHandler> values = new BindingList<IRuleHandler>();
            foreach (var item in _prototypes)
            {
                values.Add(item.Value);
            }
            return values;
        }
        public bool isExistRule(string nameRule)
        {
            if(_prototypes.ContainsKey(nameRule))
            {
                return true;    
            }
            return false;
        }
     

    }
}
