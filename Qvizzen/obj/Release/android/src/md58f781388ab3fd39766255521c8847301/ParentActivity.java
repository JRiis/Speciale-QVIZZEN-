package md58f781388ab3fd39766255521c8847301;


public class ParentActivity
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onPause:()V:GetOnPauseHandler\n" +
			"";
		mono.android.Runtime.register ("Qvizzen.ParentActivity, Qvizzen, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", ParentActivity.class, __md_methods);
	}


	public ParentActivity () throws java.lang.Throwable
	{
		super ();
		if (getClass () == ParentActivity.class)
			mono.android.TypeManager.Activate ("Qvizzen.ParentActivity, Qvizzen, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onPause ()
	{
		n_onPause ();
	}

	private native void n_onPause ();

	java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
