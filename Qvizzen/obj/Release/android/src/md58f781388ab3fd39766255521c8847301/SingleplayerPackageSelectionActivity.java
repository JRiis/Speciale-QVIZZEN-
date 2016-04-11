package md58f781388ab3fd39766255521c8847301;


public class SingleplayerPackageSelectionActivity
	extends md58f781388ab3fd39766255521c8847301.ParentActivity
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("Qvizzen.SingleplayerPackageSelectionActivity, Qvizzen, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", SingleplayerPackageSelectionActivity.class, __md_methods);
	}


	public SingleplayerPackageSelectionActivity () throws java.lang.Throwable
	{
		super ();
		if (getClass () == SingleplayerPackageSelectionActivity.class)
			mono.android.TypeManager.Activate ("Qvizzen.SingleplayerPackageSelectionActivity, Qvizzen, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);

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
