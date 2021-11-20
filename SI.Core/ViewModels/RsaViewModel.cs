using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.ViewModels;
using SI.RSAEncryption;
using System.Linq;
using System.Numerics;
using System.Text;

namespace SI.Core.ViewModels
{
    public class RsaViewModel : MvxViewModel
    {
        private RsaEncryption encryption;

        private BigInteger[] encryptedMessage;

        public IMvxCommand GenerateCommand { get; set; }
        public IMvxCommand EncryptCommand { get; set; }
        public IMvxCommand DecryptCommand { get; set; }
        public IMvxCommand ClearCommand { get; set; }

        public RsaViewModel()
        {
            GenerateCommand = new MvxCommand(Generate);
            EncryptCommand = new MvxCommand(Encrypt);
            DecryptCommand = new MvxCommand(Decrypt);
            ClearCommand = new MvxCommand(Clear);
        }

        public void Generate()
        {
            encryption = RsaEncryption.Get();
            encryptedMessage = null;
            DecryptedMessage = string.Empty;
            RaisePropertyChanged(nameof(EncryptedMessageAsString));
            RaisePropertyChanged(nameof(IsKeyGenerated));
            RaisePropertyChanged(nameof(IsEncryptEnabled));
            RaisePropertyChanged(nameof(IsDecryptEnabled));

            SetAdditionalInfo();
        }

        public void Encrypt()
        {
            encryptedMessage = encryption.Encrypt(Message);
            RaisePropertyChanged(nameof(EncryptedMessageAsString));
            RaisePropertyChanged(nameof(IsDecryptEnabled));
            RaisePropertyChanged(nameof(IsClearEnabled));
            DecryptedMessage = string.Empty;
        }

        public void Decrypt()
        {
            DecryptedMessage = encryption.Decrypt(encryptedMessage);
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

        public void SetAdditionalInfo()
        {
            StringBuilder sb = new();

            encryption.GetAdditionalInfo().ForEach(x =>
            {
                sb.AppendLine($"{ x.Key }:");
                sb.AppendLine(x.Value);
            });

            AdditionalInfo = sb.ToString();
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
                var result = strings.Aggregate((a, b) => $"{ a } { b }");
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

        private string additionalInfo;
        public string AdditionalInfo
        {
            get => additionalInfo;
            set
            {
                SetProperty(ref additionalInfo, value);
            }
        }

        public bool IsKeyGenerated => encryption != null;
        public bool IsEncryptEnabled => !string.IsNullOrEmpty(Message) && IsKeyGenerated;
        public bool IsDecryptEnabled => !string.IsNullOrEmpty(EncryptedMessageAsString);
        public bool IsClearEnabled => !string.IsNullOrEmpty(Message) || !string.IsNullOrEmpty(EncryptedMessageAsString) 
            || !string.IsNullOrEmpty(DecryptedMessage);
    }
}
