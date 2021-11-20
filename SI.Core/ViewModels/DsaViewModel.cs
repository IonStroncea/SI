using MvvmCross.Commands;
using MvvmCross.ViewModels;
using SI.DSAEncryption;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SI.Core.ViewModels
{
    public enum ValidSignatureProcess
    {
        None,
        Valid,
        Invalid
    }

    public class DsaViewModel : MvxViewModel
    {
        private DsaEncryption dsa;
        Signature signature;

        public IMvxCommand GenerateCommand { get; set; }
        public IMvxCommand SignCommand { get; set; }
        public IMvxCommand VerifyCommand { get; set; }

        public DsaViewModel()
        {
            GenerateCommand = new MvxCommand(Generate);
            SignCommand = new MvxCommand(Sign);
            VerifyCommand = new MvxCommand(Verify);

            validSignatureProcess = ValidSignatureProcess.None;
        }

        public void Generate()
        {
            dsa = DsaEncryption.Get();
            signature = null;
            validSignatureProcess = ValidSignatureProcess.None;
            RaisePropertyChanged(nameof(IsKeyGenerated));
            RaisePropertyChanged(nameof(Signature));
            RaisePropertyChanged(nameof(IsSignatureGenerated));
            RaisePropertyChanged(nameof(IsSignEnabled));
            RaisePropertyChanged(nameof(IsVerifyEnabled));
            RaisePropertyChanged(nameof(ValidSignatureMessage));
            RaisePropertyChanged(nameof(ValidationColor));

            this.SetAdditionalInfo();
        }

        public void Sign()
        {
            signature = dsa.SignData(Message);
            validSignatureProcess = ValidSignatureProcess.None;
            RaisePropertyChanged(nameof(IsSignatureGenerated));
            RaisePropertyChanged(nameof(Signature));
            RaisePropertyChanged(nameof(IsVerifyEnabled));
            RaisePropertyChanged(nameof(ValidSignatureMessage));
            RaisePropertyChanged(nameof(ValidationColor));
        }

        public void Verify()
        {
            ValidSignature = dsa.Verify(Message, signature);
        }

        public void SetAdditionalInfo()
        {
            StringBuilder sb = new();

            dsa.GetAdditionalInfo().ForEach(x =>
            {
                sb.AppendLine($"{ x.Key }: { x.Value }");
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
                RaisePropertyChanged(nameof(IsSignEnabled));
                RaisePropertyChanged(nameof(IsVerifyEnabled));
            }
        }

        public string Signature
        {
            get => IsSignatureGenerated ? $"({signature.R}, {signature.S})" : "";
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

        private ValidSignatureProcess validSignatureProcess;
        private bool validSignature;
        public bool ValidSignature
        {
            get => validSignature;
            set
            {
                SetProperty(ref validSignature, value);

                validSignatureProcess = ValidSignature ? ValidSignatureProcess.Valid : ValidSignatureProcess.Invalid; 

                RaisePropertyChanged(nameof(ValidSignatureMessage));
                RaisePropertyChanged(nameof(ValidationColor));
            }
        }
        public string ValidSignatureMessage => Enum.GetName(validSignatureProcess);
        public string ValidationColor
        {
            get
            {
                var color = "Black";

                switch (validSignatureProcess)
                {
                    case ValidSignatureProcess.Valid:
                        color = "Green";
                        break;
                    case ValidSignatureProcess.Invalid:
                        color = "Red";
                        break;
                }

                return color;
            }
        }

        public bool IsKeyGenerated => dsa != null;
        public bool IsSignatureGenerated => signature != null;
        public bool IsSignEnabled => !string.IsNullOrEmpty(Message) && IsKeyGenerated;
        public bool IsVerifyEnabled => !string.IsNullOrEmpty(Message) && IsSignatureGenerated;
    }
}
