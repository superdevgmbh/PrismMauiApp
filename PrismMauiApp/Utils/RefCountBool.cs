namespace PrismMauiApp.Utils
{
    public class RefCountBool
    {
        private readonly object syncObj = new object();

        private long refCount;

        public bool Value
        {
            get
            {
                var count = Interlocked.Read(ref this.refCount);
                return count > 0;
            }
            set
            {
                this.SetValue(value);
            }
        }

        public bool SetValue(bool value)
        {
            lock (this.syncObj)
            {
                var oldValue = this.Value;

                if (value)
                {
                    Interlocked.Increment(ref this.refCount);
                }
                else
                {
                    Interlocked.Decrement(ref this.refCount);
                }

                var newValue = this.Value;
                return oldValue != newValue;
            }
        }

        public static implicit operator bool(RefCountBool d) => d.Value;
    }
}
