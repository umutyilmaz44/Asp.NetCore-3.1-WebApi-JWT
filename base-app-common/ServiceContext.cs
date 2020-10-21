using System;
using System.Collections.Generic;

namespace base_app_common
{
    public class ServiceContext : IDisposable
    {
        public IDictionary<string, object> Items { get; } = new Dictionary<string, object>();

        public T GetItem<T>(string key)
        {
            object value;
            return Items.TryGetValue(key, out value) ? (T)value : default(T);
        }

        public void AddItem(string key, object val)
        {
            Items[key] = val;
        }

        #region Disposing
        private bool disposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {

                }
                disposed = true;
            }
        }
        ~ServiceContext()
        {
            Dispose(false);
        }
        #endregion
    }
}
