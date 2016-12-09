#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// You must obfuscate your secrets using Window > Unity IAP > Receipt Validation Obfuscator
// before receipt validation will compile in this sample.
// #define RECEIPT_VALIDATION
#endif

//heheh

#if UNITY_ANDROID || UNITY_IOS
using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;

#if RECEIPT_VALIDATION
using UnityEngine.Purchasing.Security;
#endif
public class IAP : MonoBehaviour, IStoreListener {
	public static IAP inst;
	// Unity IAP objects 
	private IStoreController m_Controller;
	private IAppleExtensions m_AppleExtensions;
	private bool m_PurchaseInProgress;
	#if RECEIPT_VALIDATION
	private CrossPlatformValidator validator;
	#endif
	void Awake(){
		inst = this;
	}
	public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
	{
		m_Controller = controller;
		m_AppleExtensions = extensions.GetExtension<IAppleExtensions> ();
		//
		m_AppleExtensions.RegisterPurchaseDeferredListener(OnDeferred);
		//通知 IAP初始化成功
//		foreach (var item in controller.products.all)
//		{
//			if (item.availableToPurchase)
//			{
//				Debug.Log(string.Join(" - ",
//					new[]
//					{
//						item.metadata.localizedTitle,
//						item.metadata.localizedDescription,
//						item.metadata.isoCurrencyCode,
//						item.metadata.localizedPrice.ToString(),
//						item.metadata.localizedPriceString
//					}));
//			}
//		}

		PlatForm.inst.GetSdk().Dispatch(Ex_Local.CALL_INIT_PIDS,"0");
	}
	public void OnInitializeFailed(InitializationFailureReason error)
	{
		Debug.Log("Billing failed to initialize!");
		string errorStr = "1000";
		switch (error)
		{
		case InitializationFailureReason.AppNotKnown:
			Debug.LogError("Is your App correctly uploaded on the relevant publisher console?");
			errorStr = "1000";
			break;
		case InitializationFailureReason.PurchasingUnavailable:
			// Ask the user if billing is disabled in device settings.
			Debug.Log("Billing disabled!");
			errorStr = "1002";
			break;
		case InitializationFailureReason.NoProductsAvailable:
			// Developer configuration error; check product metadata.
			Debug.Log("No products available for purchase!");
			errorStr = "1003";
			break;
		}
		PlatForm.inst.GetSdk().Dispatch(Ex_Local.CALL_INIT_PIDS,errorStr);

	}
	/// <summary>
	/// This will be called after a call to IAppleExtensions.RestoreTransactions().
	/// </summary>
	private void OnTransactionsRestored(bool success)
	{
		Debug.Log("--->> Transactions restored.");
	}
	/// <summary>
	/// iOS Specific.
	/// This is called as part of Apple's 'Ask to buy' functionality,
	/// when a purchase is requested by a minor and referred to a parent
	/// for approval.
	/// 
	/// When the purchase is approved or rejected, the normal purchase events
	/// will fire.
	/// </summary>
	/// <param name="item">Item.</param>
	private void OnDeferred(Product item)
	{
		Debug.Log ("--->> Purchase deferred: " + item.definition.id + " || " + item.definition.storeSpecificId);
	}
	/// <summary>
	/// This will be called when a purchase completes.
	/// </summary>
	public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
	{
		
		Debug.Log("--->> Purchase OK: " + e.purchasedProduct.definition.id + " || " + e.purchasedProduct.definition.storeSpecificId);
		Debug.Log("--->> Receipt: " + e.purchasedProduct.receipt);

		m_PurchaseInProgress = false;

		PlatForm.inst.GetSdk ().Dispatch (Ex_Local.CALL_BUY, new string[]{e.purchasedProduct.receipt,e.purchasedProduct.definition.id});

//		#if RECEIPT_VALIDATION
//		if (Application.platform == RuntimePlatform.Android ||
//			Application.platform == RuntimePlatform.IPhonePlayer ||
//			Application.platform == RuntimePlatform.OSXPlayer) 
//		{
//			try {
//				var result = validator.Validate(e.purchasedProduct.receipt);
//				Debug.Log("Receipt is valid. Contents:");
//				foreach (IPurchaseReceipt productReceipt in result) {
//					Debug.Log(productReceipt.productID);
//					Debug.Log(productReceipt.purchaseDate);
//					Debug.Log(productReceipt.transactionID);
//
//					GooglePlayReceipt google = productReceipt as GooglePlayReceipt;
//					if (null != google) {
//						Debug.Log(google.purchaseState);
//						Debug.Log(google.purchaseToken);
//					}
//
//					AppleInAppPurchaseReceipt apple = productReceipt as AppleInAppPurchaseReceipt;
//					if (null != apple) {
//						Debug.Log(apple.originalTransactionIdentifier);
//						Debug.Log(apple.cancellationDate);
//						Debug.Log(apple.quantity);
//					}
//				}
//			} catch (IAPSecurityException) {
//			Debug.Log("Invalid receipt, not unlocking content");
//			return PurchaseProcessingResult.Complete;
//			}
//		}
//		#endif
		return PurchaseProcessingResult.Complete;
	}
	/// <summary>
	/// This will be called is an attempted purchase fails.
	/// </summary>
	public void OnPurchaseFailed(Product item, PurchaseFailureReason r)
	{
		Debug.Log("--->> Purchase failed: " + item.definition.id);
		Debug.Log("--->> "+r);
		string errorStr = "2000";
		switch (r) 
		{
		case PurchaseFailureReason.UserCancelled:
			errorStr = "2000";
			break;
		case PurchaseFailureReason.ExistingPurchasePending:
			errorStr = "2005";
			break;
		case PurchaseFailureReason.PaymentDeclined:
			errorStr = "2004";
			break;
		case PurchaseFailureReason.ProductUnavailable:
			break;
		case PurchaseFailureReason.PurchasingUnavailable:
			errorStr = "2003";
			break;
		case PurchaseFailureReason.SignatureInvalid:
			errorStr = "2002";
			break;
		case PurchaseFailureReason.Unknown:
			errorStr = "2001";
			break;
		}

		m_PurchaseInProgress = false;

		PlatForm.inst.GetSdk ().Dispatch (Ex_Local.CALL_BUY, errorStr);//
	}
	public void InitProduct(string[] pids,string androidKEY){

		var module = StandardPurchasingModule.Instance();

		// The FakeStore supports: no-ui (always succeeding), basic ui (purchase pass/fail), and 
		// developer ui (initialization, purchase, failure code setting). These correspond to 
		// the FakeStoreUIMode Enum values passed into StandardPurchasingModule.useFakeStoreUIMode.
		module.useFakeStoreUIMode = FakeStoreUIMode.StandardUser;

		var builder = ConfigurationBuilder.Instance(module);
		// This enables the Microsoft IAP simulator for local testing.
		// You would remove this before building your release package.
		builder.Configure<IMicrosoftConfiguration>().useMockBillingSystem = true;
//		builder.Configure<IGooglePlayConfiguration>().SetPublicKey("MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA2O/9/H7jYjOsLFT/uSy3ZEk5KaNg1xx60RN7yWJaoQZ7qMeLy4hsVB3IpgMXgiYFiKELkBaUEkObiPDlCxcHnWVlhnzJBvTfeCPrYNVOOSJFZrXdotp5L0iS2NVHjnllM+HA1M0W2eSNjdYzdLmZl1bxTpXa4th+dVli9lZu7B7C2ly79i/hGTmvaClzPBNyX+Rtj7Bmo336zh2lYbRdpD5glozUq+10u91PMDPH+jqhx10eyZpiapr8dFqXl5diMiobknw9CgcjxqMTVBQHK6hS0qYKPmUDONquJn280fBs1PTeA6NMG03gb9FLESKFclcuEZtvM8ZwMMRxSLA9GwIDAQAB");
		builder.Configure<IGooglePlayConfiguration>().SetPublicKey(androidKEY);

		// Define our products.
		// In this case our products have the same identifier across all the App stores,
		// except on the Mac App store where product IDs cannot be reused across both Mac and
		// iOS stores.
		// So on the Mac App store our products have different identifiers,
		// and we tell Unity IAP this by using the IDs class.

		string pName = "";

		if (Application.platform == RuntimePlatform.Android) {
			pName = GooglePlay.Name;
		} else if (Application.platform == RuntimePlatform.IPhonePlayer) {
			pName = AppleAppStore.Name;
		} else {
			pName = "";
		}
		if (pids != null && pids.Length > 0) {
			for (int i = 0; i < pids.Length; i++) {
				builder.AddProduct(pids[i], ProductType.Consumable, new IDs
					{
						{pids[i], pName},
					});
			}

		}
		// Write Amazon's JSON description of our products to storage when using Amazon's local sandbox.
		// This should be removed from a production build.
		builder.Configure<IAmazonConfiguration>().WriteSandboxJSON(builder.products);

		#if RECEIPT_VALIDATION
		validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.bundleIdentifier);
		#endif

		// Now we're ready to initialize Unity IAP.
		UnityPurchasing.Initialize(this, builder);
		
	}
	public void Buy(string pid){
		if (m_Controller == null) {
			Debug.Log ("--->> errror m_Controller == null"); 
			return;
		}
		if (m_PurchaseInProgress == true) {
			return;
		}
		Product selected = CheckProduct (pid);
		if (selected != null) {
			m_Controller.InitiatePurchase (CheckProduct (pid)); 
			// Don't need to draw our UI whilst a purchase is in progress.
			// This is not a requirement for IAP Applications but makes the demo
			// scene tidier whilst the fake purchase dialog is showing.
			m_PurchaseInProgress = true;
		} else {
			Debug.Log ("--->> Buy() 没有找到支付产品对象 selected == null");
			m_PurchaseInProgress = false;
		}
	}
	public void RestoreTransactions(){
		if (m_AppleExtensions != null) {
			m_AppleExtensions.RestoreTransactions (OnTransactionsRestored);
		}
	}
	private Product CheckProduct(string pid){
		Product selected = null;
		if (m_Controller != null) {
			for (int i = 0; i < m_Controller.products.all.Length; i++) {
				selected = m_Controller.products.all [i];
				if (pid == selected.definition.id) {
					break;
				}
			}
		}
		return selected;
	}
}
#endif
