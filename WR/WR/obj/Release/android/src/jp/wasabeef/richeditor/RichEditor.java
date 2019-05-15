package jp.wasabeef.richeditor;


public class RichEditor
	extends android.webkit.WebView
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_setPadding:(IIII)V:GetSetPadding_IIIIHandler\n" +
			"n_setPaddingRelative:(IIII)V:GetSetPaddingRelative_IIIIHandler\n" +
			"n_setBackgroundResource:(I)V:GetSetBackgroundResource_IHandler\n" +
			"n_getBackground:()Landroid/graphics/drawable/Drawable;:GetGetBackgroundHandler\n" +
			"n_setBackground:(Landroid/graphics/drawable/Drawable;)V:GetSetBackground_Landroid_graphics_drawable_Drawable_Handler\n" +
			"";
		mono.android.Runtime.register ("Jp.Wasabeef.RichEditor, RichEditor", RichEditor.class, __md_methods);
	}


	public RichEditor (android.content.Context p0, android.util.AttributeSet p1)
	{
		super (p0, p1);
		if (getClass () == RichEditor.class)
			mono.android.TypeManager.Activate ("Jp.Wasabeef.RichEditor, RichEditor", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android", this, new java.lang.Object[] { p0, p1 });
	}


	public RichEditor (android.content.Context p0, android.util.AttributeSet p1, int p2)
	{
		super (p0, p1, p2);
		if (getClass () == RichEditor.class)
			mono.android.TypeManager.Activate ("Jp.Wasabeef.RichEditor, RichEditor", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android:System.Int32, mscorlib", this, new java.lang.Object[] { p0, p1, p2 });
	}


	public void setPadding (int p0, int p1, int p2, int p3)
	{
		n_setPadding (p0, p1, p2, p3);
	}

	private native void n_setPadding (int p0, int p1, int p2, int p3);


	public void setPaddingRelative (int p0, int p1, int p2, int p3)
	{
		n_setPaddingRelative (p0, p1, p2, p3);
	}

	private native void n_setPaddingRelative (int p0, int p1, int p2, int p3);


	public void setBackgroundResource (int p0)
	{
		n_setBackgroundResource (p0);
	}

	private native void n_setBackgroundResource (int p0);


	public android.graphics.drawable.Drawable getBackground ()
	{
		return n_getBackground ();
	}

	private native android.graphics.drawable.Drawable n_getBackground ();


	public void setBackground (android.graphics.drawable.Drawable p0)
	{
		n_setBackground (p0);
	}

	private native void n_setBackground (android.graphics.drawable.Drawable p0);

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
