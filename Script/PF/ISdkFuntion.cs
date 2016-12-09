using System;

public interface ISdkFuntion
{
	void Init (string type);

	void Init (string className, string method);

	void InitPF (string type);

	void Login(string type,bool change = false);

	void Clear ();

	void Call (string method, Action<object> fun);

	void Call (string method, object[] args, Action<object> fun);

	string CallReturn(string method);

	void Dispatch (string method, object data);

	void CallBuy(string pid, Action<object> fun);

	void CallInitPIDs(string[] pids,string androidKEY, Action<object> fun);
}