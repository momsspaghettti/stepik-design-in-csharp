using System;


namespace Memory.API {
    public class APIObject : IDisposable {
        private readonly int _id;
        private bool _isDisposed;

        public APIObject(int id) {
            _id = id;
            MagicAPI.Allocate(id);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool fromDispose) {
            if (!_isDisposed) {
                MagicAPI.Free(_id);
                _isDisposed = true;
            }
        }

        ~APIObject() {
            Dispose(false);
        }
    }
}