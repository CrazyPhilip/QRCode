using QRCode.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace QRCode.ViewModels
{
    public class ScanViewModel : BaseViewModel
    {
        private ProductInfo product;   //Comment
        public ProductInfo Product
        {
            get { return product; }
            set { SetProperty(ref product, value); }
        }

        private string productName;   //Comment
        public string ProductName
        {
            get { return productName; }
            set { SetProperty(ref productName, value); }
        }

        private string weight;   //Comment
        public string Weight
        {
            get { return weight; }
            set { SetProperty(ref weight, value); }
        }

        private string recipientName;   //Comment
        public string RecipientName
        {
            get { return recipientName; }
            set { SetProperty(ref recipientName, value); }
        }

        private string recipientPhone;   //Comment
        public string RecipientPhone
        {
            get { return recipientPhone; }
            set { SetProperty(ref recipientPhone, value); }
        }

        private string recipientAddress;   //Comment
        public string RecipientAddress
        {
            get { return recipientAddress; }
            set { SetProperty(ref recipientAddress, value); }
        }

        private string senderName;   //Comment
        public string SenderName
        {
            get { return senderName; }
            set { SetProperty(ref senderName, value); }
        }

        private string senderPhone;   //Comment
        public string SenderPhone
        {
            get { return senderPhone; }
            set { SetProperty(ref senderPhone, value); }
        }

        private string senderAddress;   //Comment
        public string SenderAddress
        {
            get { return senderAddress; }
            set { SetProperty(ref senderAddress, value); }
        }
    }
}
