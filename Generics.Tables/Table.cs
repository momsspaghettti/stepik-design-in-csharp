using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Generics.Tables {
    public class Table<TRows, TColumns, TItems> {
        private readonly TItems[,] _items = new TItems[1000, 1000];
        private readonly Dictionary<TRows, int> _rows = new Dictionary<TRows, int>();
        private int _rowsCounter;
        private readonly Dictionary<TColumns, int> _columns = new Dictionary<TColumns, int>();
        private int _columnsCounter;

        public struct Counts {
            public int count;

            public int Count() {
                return count;
            }
        }

        private Counts _rowsCount;
        public Counts Rows => _rowsCount;

        private Counts _columnsCount;
        public Counts Columns => _columnsCount;

        private bool addRow(TRows rowItem) {
            if (!_rows.ContainsKey(rowItem)) {
                _rows[rowItem] = _rowsCounter;
                ++_rowsCounter;
                return true;
            }

            return false;
        }

        public void AddRow(TRows rowItem) {
            if (addRow(rowItem))
                ++_rowsCount.count;
        }

        private bool addColumn(TColumns columnItem) {
            if (!_columns.ContainsKey(columnItem)) {
                _columns[columnItem] = _columnsCounter;
                ++_columnsCounter;
                return true;
            }

            return false;
        }

        public void AddColumn(TColumns columnItem) {
            if (addColumn(columnItem))
                ++_columnsCount.count;
        }

        public Accessor<TRows, TColumns, TItems> Open =>
            new Accessor<TRows, TColumns, TItems>(this, true);

        public Accessor<TRows, TColumns, TItems> Existed =>
            new Accessor<TRows, TColumns, TItems>(this, false);

        public class Accessor<TRows, TColumns, TItems> {
            private bool _open;
            private Table<TRows, TColumns, TItems> _owner;

            public Accessor(Table<TRows, TColumns, TItems> owner, bool open) {
                _open = open;
                _owner = owner;
            }

            private bool AddRowsColumnsIfNeeded(TRows row, TColumns column) {
                var rowAdded = _owner.addRow(row);
                var columnAdded = _owner.addColumn(column);
                return rowAdded || columnAdded;
            }

            public TItems this[TRows row, TColumns column] {
                get {
                    if (_open)
                        if (AddRowsColumnsIfNeeded(row, column))
                            _owner._items[_owner._rows[row], _owner._columns[column]] = default;
                    try {
                        return _owner._items[_owner._rows[row], _owner._columns[column]];
                    }
                    catch (Exception) {
                        throw new ArgumentException();
                    }
                }
                set {
                    if (_open) {
                        _owner.AddRow(row);
                        _owner.AddColumn(column);
                    }

                    try {
                        _owner._items[_owner._rows[row], _owner._columns[column]] = value;
                    }
                    catch (Exception) {
                        throw new ArgumentException();
                    }
                }
            }
        }
    }
}