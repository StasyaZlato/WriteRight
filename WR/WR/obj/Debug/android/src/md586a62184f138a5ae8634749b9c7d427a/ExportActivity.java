package md586a62184f138a5ae8634749b9c7d427a;


public class ExportActivity
	extends md586a62184f138a5ae8634749b9c7d427a.MainActivity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("WR.Activities.ExportActivity, WR", ExportActivity.class, __md_methods);
	}


	public ExportActivity ()
	{
		super ();
		if (getClass () == ExportActivity.class)
			mono.android.TypeManager.Activate ("WR.Activities.ExportActivity, WR", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);

	private java.util.ArrayList refList;
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
