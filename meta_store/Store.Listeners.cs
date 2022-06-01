using System;
using System.Collections.Generic;

namespace meta_store
{
    // TODO child biết parent, nhưng parent không cần biết child
    // ISSUE: should handler receive current values or waiting for the next value?
    public partial class Store
    {
        private readonly object diff;
        private HashSet<Action<object>> listeners;
        private readonly HashSet<Action<object>> childListeners;
        internal int listenerCount;

        internal int dirty;


        public void AddListener(Action<object> action)
        {
            // TODO send first value? state? last_sent?
            // TODO unsubcriber
            // TODO allow duplication
            // TODO first listener: sent == null;
            listeners = listeners ?? new HashSet<Action<object>>();
            if (listeners.Add(action))
            {
                var p = this;
                while (p != null)
                {
                    p.listenerCount++;
                    p = p.parent;
                }
            }
        }

        public void RemoveListener(Action<object> action)
        {
            // TODO last listener: sent = null
            if (listeners.Remove(action))
            {
                var p = this;
                while (p != null)
                {
                    p.listenerCount--;
                    p = p.parent;
                }
            }
        }

        private bool HasListeners() => listenerCount > 0;

        private bool HasBeenChanged() => dirty > 0;

        private bool NeedUpdate() => HasListeners() && HasBeenChanged();

        // event
        // - cach 1: compare with last sent
        // - cach 2:  mark as handlers to be sent
        public void Update()
        {
            if (NeedUpdate())
            {
                foreach (var child in children)
                {
                    child.Value.Update();
                }
            }
        }
    }
}
