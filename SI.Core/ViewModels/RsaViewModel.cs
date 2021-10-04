using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using SI.RSAEncryption;
using System;
using System.Linq;
using System.Numerics;
using System.Windows;

namespace SI.Core.ViewModels
{
    public class RsaViewModel : MvxViewModel
    {
        private readonly IMvxNavigationService mvxNavigationService;

        private Rsa rsa;

        private BigInteger[] encryptedMessage;

        public IMvxCommand GenerateKeyCommand { get; set; }
        public IMvxCommand EncryptCommand { get; set; }
        public IMvxCommand DecryptCommand { get; set; }
        public IMvxCommand ClearCommand { get; set; }

        public RsaViewModel(IMvxNavigationService mvxNavigationService)
        {
            this.mvxNavigationService = mvxNavigationService;
            GenerateKeyCommand = new MvxCommand(GenerateKey);
            EncryptCommand = new MvxCommand(Encrypt);
            DecryptCommand = new MvxCommand(Decrypt);
            ClearCommand = new MvxCommand(Clear);
        }

        public void GenerateKey()
        {
            rsa = EncryptionGenerator.GetRsaEncryption();
            encryptedMessage = null;
            DecryptedMessage = string.Empty;
            RaisePropertyChanged(nameof(EncryptedMessageAsString));
            RaisePropertyChanged(nameof(IsKeyGenerated));
            RaisePropertyChanged(nameof(IsEncryptEnabled));
            RaisePropertyChanged(nameof(IsDecryptEnabled));
            RaisePropertyChanged(nameof(PrivateKey));
            RaisePropertyChanged(nameof(PublicKey));
        }

        public void Encrypt()
        {
            encryptedMessage = rsa.Encrypt(Message);
            RaisePropertyChanged(nameof(EncryptedMessageAsString));
            RaisePropertyChanged(nameof(IsDecryptEnabled));
            RaisePropertyChanged(nameof(IsClearEnabled));
            DecryptedMessage = string.Empty;
        }

        public void Decrypt()
        {
            DecryptedMessage = rsa.Decrypt(encryptedMessage);
            RaisePropertyChanged(nameof(IsClearEnabled));
        }

        public void Clear()
        {
            Message = string.Empty;
            encryptedMessage = null;
            DecryptedMessage = string.Empty;
            RaisePropertyChanged(nameof(Message));
            RaisePropertyChanged(nameof(EncryptedMessageAsString));
            RaisePropertyChanged(nameof(DecryptedMessage));
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

        public string PublicKey => IsKeyGenerated ? $"({ rsa.E }, { rsa.N })" : "";
        public string PrivateKey => IsKeyGenerated ? $"({ rsa.D }, { rsa.N })" : "";

        public bool IsKeyGenerated => rsa != null;
        public bool IsEncryptEnabled => !string.IsNullOrEmpty(Message) && IsKeyGenerated;
        public bool IsDecryptEnabled => !string.IsNullOrEmpty(EncryptedMessageAsString);
        public bool IsClearEnabled => !string.IsNullOrEmpty(Message) || !string.IsNullOrEmpty(EncryptedMessageAsString) 
            || !string.IsNullOrEmpty(DecryptedMessage);
    }
}
