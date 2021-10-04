using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using SI.RSAEncryption;
using System;
using System.Linq;
using System.Numerics;

namespace SI.Core.ViewModels
{
    public class RsaViewModel : MvxViewModel
    {
        private readonly IMvxNavigationService mvxNavigationService;

        private Rsa rsa;

        private BigInteger[] encryptedMessage;

        public IMvxCommand EncryptCommand { get; set; }
        public IMvxCommand DecryptCommand { get; set; }
        public IMvxCommand ClearCommand { get; set; }

        public RsaViewModel(IMvxNavigationService mvxNavigationService)
        {
            this.mvxNavigationService = mvxNavigationService;
            EncryptCommand = new MvxCommand(Encrypt);
            DecryptCommand = new MvxCommand(Decrypt);
            ClearCommand = new MvxCommand(Clear);

            rsa = KeyGenerator.GetRsaEncryption();
        }

        public void Encrypt()
        {
            encryptedMessage = rsa.Encrypt(Message);
            RaisePropertyChanged(nameof(EncryptedMessageAsString));
            RaisePropertyChanged(nameof(IsDecryptEnabled));
            RaisePropertyChanged(nameof(IsClearEnabled));
        }

        public void Decrypt()
        {
            DecryptedMessage = rsa.Decrypt(encryptedMessage);
            RaisePropertyChanged(nameof(IsClearEnabled));
        }

        public void Clear()
        {
            Message = string.Empty;
            encryptedMessage = new BigInteger[0];
            DecryptedMessage = string.Empty;
            RaisePropertyChanged(nameof(EncryptedMessageAsString));
            RaisePropertyChanged(nameof(IsDecryptEnabled));
            RaisePropertyChanged(nameof(IsClearEnabled));
        }

        private string message;
        public string Message
        {
            get => message;
            set
            {
                SetProperty(ref message, value);
                RaisePropertyChanged(nameof(IsEncryptEnabled));
            }
        }

        public string EncryptedMessageAsString
        {
            get {
                if (encryptedMessage == null || encryptedMessage.Length == 0) return string.Empty;
                var strings = encryptedMessage.ToList().Select(x => x.ToString());
                var result = strings.Aggregate((a, b) => a + b);
                return result;
            }
        }

        private string decryptedMessage;
        public string DecryptedMessage
        {
            get => decryptedMessage;
            set
            {
                SetProperty(ref decryptedMessage, value);
            }
        }

        public bool IsEncryptEnabled => !string.IsNullOrEmpty(Message);
        public bool IsDecryptEnabled => !string.IsNullOrEmpty(EncryptedMessageAsString);
        public bool IsClearEnabled => !string.IsNullOrEmpty(Message) || !string.IsNullOrEmpty(EncryptedMessageAsString) 
            || !string.IsNullOrEmpty(DecryptedMessage);
    }
}
