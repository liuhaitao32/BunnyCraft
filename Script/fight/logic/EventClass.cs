using System;
using System.Collections.Generic;

public class EventClass : IClear {
    private DispatchManager instance;

    protected bool _cleared = false;

    public EventClass() {

    }

	public void addListener(string name, Action<MainEvent> fun) {
        if(instance == null)
            instance = new DispatchManager();
        instance.Register(name, fun);
    }

	public void removeListener(string name, Action<MainEvent> fun) {
        if(instance == null)
            return;
        instance.Unregister(name, fun);
    }

    public void removeListeners(string name) {
        if(instance == null)
            return;
        instance.Unregisters(name);
    }

    public bool hasEventListener(string name) {
        return null != instance && instance.HasEventListener(name);
    }

    public void removeListenerAll() {
        if(instance == null)
            return;
        instance.Clear();
    }

    public void dispatchEvent(MainEvent e) {
        e.target = this;
        if(null != instance)
            instance.Dispatch(e);
    }

    public void dispatchEventWith(string name, object data = null) {
        if(null != instance) {
            MainEvent me = new MainEvent(name, data);
            me.target = this;
            instance.Dispatch(me);
        }
    }


    public bool cleared { get { return this._cleared; } }

    public virtual void clear() {
        this._cleared = true;
        this.removeListenerAll();
    }

}