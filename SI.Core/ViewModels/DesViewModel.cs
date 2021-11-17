using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using SI.Common;
using SI.DESEncryption;
using SI.RSAEncryption;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Windows;

namespace SI.Core.ViewModels
{
    public class DesViewModel : MvxViewModel
    {
        List<long> encryptedMessage;

        private DesEncryption encryption;

        public IMvxCommand GenerateCommand { get; set; }
        public IMvxCommand EncryptCommand { get; set; }
        public IMvxCommand DecryptCommand { get; set; }
        public IMvxCommand ClearCommand { get; set; }

        public DesViewModel()
        {
            GenerateCommand = new MvxCommand(Generate);
            EncryptCommand = new MvxCommand(Encrypt);
            DecryptCommand = new MvxCommand(Decrypt);
            ClearCommand = new MvxCommand(Clear);

            encryptedMessage = new();
        }

        public void Generate()
        {
            encryption = DesEncryption.Get();
            encryptedMessage.Clear();
            DecryptedMessage = string.Empty;
            RaisePropertyChanged(nameof(IsKeyGenerated));
            RaisePropertyChanged(nameof(EncryptedMessage));
            RaisePropertyChanged(nameof(IsEncryptEnabled));
            RaisePropertyChanged(nameof(DecryptedMessage));
            RaisePropertyChanged(nameof(IsClearEnabled));

            this.SetAdditionalInfo();
        }

        public void Encrypt()
        {
            encryptedMessage = encryption.Encrypt(Message);
            RaisePropertyChanged(nameof(EncryptedMessage));
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
            encryptedMessage.Clear();
            DecryptedMessage = string.Empty;
            RaisePropertyChanged(nameof(EncryptedMessage));
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

        public string EncryptedMessage
        {
            get 
            { 
                if (encryptedMessage.Count == 0) return string.Empty;

                var result = "";
                var byteList = DesEncryption.LongListToByteList(encryptedMessage);

                byteList.ForEach((value) =>
                {
                    result += value.ToString("X").PadLeft(2, '0');
                });

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
        public bool IsDecryptEnabled => !string.IsNullOrEmpty(EncryptedMessage);
        public bool IsClearEnabled => !string.IsNullOrEmpty(Message) || !string.IsNullOrEmpty(EncryptedMessage) 
            || !string.IsNullOrEmpty(DecryptedMessage);
    }
}
