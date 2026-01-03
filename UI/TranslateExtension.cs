using System;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows;
using PCL.Core.App;

namespace PCL.Core.UI
{
    public class TranslateExtension : MarkupExtension
    {
        public string Key { get; set; }

        public TranslateExtension(string key)
        {
            Key = key;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var translationData = new TranslationData { Key = this.Key };
            
            // Subscribe to language changes
            I18nService.LanguageChanged += (sender, e) => {
                translationData.Refresh();
            };
            
            var binding = new Binding
            {
                Source = translationData,
                Path = new PropertyPath("Value"),
                Mode = BindingMode.OneWay
            };
            return binding.ProvideValue(serviceProvider);
        }
    }

    public class TranslationData : PCL.Core.App.TranslationNotifier
    {
        private string _key;
        
        public string Key
        {
            get { return _key; }
            set
            {
                _key = value;
                Refresh();
            }
        }
        
        public string Value
        {
            get { return I18nService.Get(Key); }
        }
    }
}