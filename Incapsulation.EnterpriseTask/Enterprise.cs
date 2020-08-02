using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incapsulation.EnterpriseTask {
    public class Enterprise {
        private readonly Guid _guid;

        public Guid Guid {
            get => _guid;
        }

        public Enterprise(Guid guid) {
            this._guid = guid;
        }

        public string Name { get; set; }

        private string _inn;

        public string Inn {
            get => _inn;
            set {
                if (value.Length != 10 || !value.All(char.IsDigit))
                    throw new ArgumentException();
                _inn = value;
            }
        }

        public DateTime EstablishDate { get; set; }

        public TimeSpan ActiveTimeSpan {
            get => DateTime.Now - EstablishDate;
        }

        public double GetTotalTransactionsAmount() {
            DataBase.OpenConnection();
            var amount = 0.0;
            foreach (Transaction t in DataBase.Transactions().Where(z => z.EnterpriseGuid == _guid))
                amount += t.Amount;
            return amount;
        }
    }
}