using System;
using System.Collections.Generic;
using UnityEngine;

public class FightViewBase : MonoBehaviour, IClear {
    private DispatchManager instance;

    protected bool _cleared = false;

    protected bool _isEnabled = false;

    protected Transform _transform;

    public FightViewBase() {

    }

    void Awake() {
        this.preInit();
    }

    protected virtual void preInit() {
        this._isEnabled = true;
        this._transform = this.transform;
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

    public void DispatchEvent(MainEvent e) {
        if(null != instance)
            instance.Dispatch(e);
    }

    public void DispatchEventWith(string name, object data = null) {
        if(null != instance) {
            MainEvent me = new MainEvent(name, data);
            instance.Dispatch(me);
        }
    }


    public void removeListeners(string name) {
        if(instance == null)
            return;
        instance.Unregisters(name);
    }

    public void removeListenerAll() {
        if(instance == null)
            return;
        instance.Clear();
    }

    public bool cleared { get { return this._cleared; } }

    public virtual void OnDestroy() {
        this._isEnabled = false;
        Utils.clearObject(this);
    }

    public virtual void clear() {
        this._cleared = true;
        this.removeListenerAll();
        if(this._isEnabled) Destroy(this.gameObject);
        this._isEnabled = false;
    }
}