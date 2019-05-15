package jp.wasabeef.richeditor;


public class RichEditor_EditorWebViewClient
	extends android.webkit.WebViewClient
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onPageFinished:(Landroid/webkit/WebView;Ljava/lang/String;)V:GetOnPageFinished_Landroid_webkit_WebView_Ljava_lang_String_Handler\n" +
			"n_shouldOverrideUrlLoading:(Landroid/webkit/WebView;Landroid/webkit/WebResourceRequest;)Z:GetShouldOverrideUrlLoading_Landroid_webkit_WebView_Landroid_webkit_WebResourceRequest_Handler\n" +
			"n_shouldOverrideUrlLoading:(Landroid/webkit/WebView;Ljava/lang/String;)Z:GetShouldOverrideUrlLoading_Landroid_webkit_WebView_Ljava_lang_String_Handler\n" +
			"";
		mono.android.Runtime.register ("Jp.Wasabeef.RichEditor+EditorWebViewClient, RichEditor", RichEditor_EditorWebViewClient.class, __md_methods);
	}


	public RichEditor_EditorWebViewClient ()
	{
		super ();
		if (getClass () == RichEditor_EditorWebViewClient.class)
			mono.android.TypeManager.Activate ("Jp.Wasabeef.RichEditor+EditorWebViewClient, RichEditor", "", this, new java.lang.Object[] {  });
	}

	public RichEditor_EditorWebViewClient (jp.wasabeef.richeditor.RichEditor p0)
	{
		super ();
		if (getClass () == RichEditor_EditorWebViewClient.class)
			mono.android.TypeManager.Activate ("Jp.Wasabeef.RichEditor+EditorWebViewClient, RichEditor", "Jp.Wasabeef.RichEditor, RichEditor", this, new java.lang.Object[] { p0 });
	}


	public void onPageFinished (android.webkit.WebView p0, java.lang.String p1)
	{
		n_onPageFinished (p0, p1);
	}

	private native void n_onPageFinished (android.webkit.WebView p0, java.lang.String p1);

	@android.annotation.TargetApi(
value = 24)

	public boolean shouldOverrideUrlLoading (android.webkit.WebView p0, android.webkit.WebResourceRequest p1)
	{
		return n_shouldOverrideUrlLoading (p0, p1);
	}

	private native boolean n_shouldOverrideUrlLoading (android.webkit.WebView p0, android.webkit.WebResourceRequest p1);


	public boolean shouldOverrideUrlLoading (android.webkit.WebView p0, java.lang.String p1)
	{
		return n_shouldOverrideUrlLoading (p0, p1);
	}

	private native boolean n_shouldOverrideUrlLoading (android.webkit.WebView p0, java.lang.String p1);

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
