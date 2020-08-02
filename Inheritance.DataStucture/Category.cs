using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inheritance.DataStructure {
    public class Category : IComparable {
        private readonly string _messageName;
        private readonly MessageType _messageType;
        private readonly MessageTopic _messageTopic;

        public Category(string messageName, MessageType messageType, MessageTopic messageTopic) {
            _messageName = messageName;
            _messageType = messageType;
            _messageTopic = messageTopic;
        }

        public override bool Equals(object obj) {
            try {
                var other = (Category) obj;
                return string.Equals(_messageName, other._messageName) &&
                       _messageType == other._messageType &&
                       _messageTopic == other._messageTopic;
            }
            catch (Exception) {
                return false;
            }
        }

        public override int GetHashCode() {
            int k = 137;
            return k * k * _messageName.GetHashCode() + k * (int) _messageType + (int) _messageTopic;
        }

        public override string ToString() {
            return $"{_messageName}.{_messageType.ToString("G")}.{_messageTopic.ToString("G")}";
        }

        public int CompareTo(object obj) {
            if (obj == null)
                return 1;
            var other = (Category) obj;
            int first = string.Compare(_messageName, other._messageName);
            if (first != 0)
                return first;
            int second = (int) _messageType - (int) other._messageType;
            if (second != 0)
                return second;
            return (int) _messageTopic - (int) other._messageTopic;
        }

        public static bool operator <(Category l, Category r) {
            return l.CompareTo(r) < 0;
        }

        public static bool operator >(Category l, Category r) {
            return l.CompareTo(r) > 0;
        }

        public static bool operator <=(Category l, Category r) {
            return !(l > r);
        }

        public static bool operator >=(Category l, Category r) {
            return !(l < r);
        }
    }
}