using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LndrMeWin8App
{
    class ClaimApplianceCommand : ICommand
    {
        protected MainViewModel mvm;
        protected ApplianceViewModel avm;

        public ClaimApplianceCommand(MainViewModel mvm, ApplianceViewModel avm)
        {
            this.mvm = mvm;
            this.avm = avm;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object prameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            mvm.OnClaim(avm);
        }
    }
}
