package md5f8922b1657a79041ee5b570f5a24eb67;


public class ViewHolderRemoving
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("WR.CustomViews.ViewHolderRemoving, WR", ViewHolderRemoving.class, __md_methods);
	}


	public ViewHolderRemoving ()
	{
		super ();
		if (getClass () == ViewHolderRemoving.class)
			mono.android.TypeManager.Activate ("WR.CustomViews.ViewHolderRemoving, WR", "", this, new java.lang.Object[] {  });
	}

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
