package md5678e1112c07d506c4787555b97aefc0f;


public class PackageCreatorQuestionActivity
	extends md58f781388ab3fd39766255521c8847301.ParentActivity
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"n_onResume:()V:GetOnResumeHandler\n" +
			"";
		mono.android.Runtime.register ("Qvizzen.Activities.PackageCreatorQuestionActivity, Qvizzen, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", PackageCreatorQuestionActivity.class, __md_methods);
	}


	public PackageCreatorQuestionActivity () throws java.lang.Throwable
	{
		super ();
		if (getClass () == PackageCreatorQuestionActivity.class)
			mono.android.TypeManager.Activate ("Qvizzen.Activities.PackageCreatorQuestionActivity, Qvizzen, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);


	public void onResume ()
	{
		n_onResume ();
	}

	private native void n_onResume ();

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
