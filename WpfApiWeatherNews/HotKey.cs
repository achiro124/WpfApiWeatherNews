using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfApiWeatherNews
{
    [Serializable]
    public class HotKey : INotifyPropertyChanged, ISerializable, IEquatable<HotKey>
    {
        public HotKey() { }

        /// <summary>
        /// Creates an HotKey object. This instance has to be registered in an HotKeyHost.
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="modifiers">The modifier. Multiple modifiers can be combined with or.</param>
        public HotKey(Key key, ModifierKeys modifiers) : this(key, modifiers, true) { }

        /// <summary>
        /// Creates an HotKey object. This instance has to be registered in an HotKeyHost.
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="modifiers">The modifier. Multiple modifiers can be combined with or.</param>
        /// <param name="enabled">Specifies whether the HotKey will be enabled when registered to an HotKeyHost</param>
        public HotKey(Key key, ModifierKeys modifiers, bool enabled)
        {
            Key = key;
            Modifiers = modifiers;
            Enabled = enabled;
        }


        private Key key;

        public Key Key
        {
            get { return key; }
            set
            {
                if (key != value)
                {
                    key = value;
                    OnPropertyChanged("Key");
                }
            }
        }

        private ModifierKeys modifiers;

        public ModifierKeys Modifiers
        {
            get { return modifiers; }
            set
            {
                if (modifiers != value)
                {
                    modifiers = value;
                    OnPropertyChanged("Modifiers");
                }
            }
        }

        private bool enabled;
        public bool Enabled
        {
            get { return enabled; }
            set
            {
                if (value != enabled)
                {
                    enabled = value;
                    OnPropertyChanged("Enabled");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }


        public override bool Equals(object obj)
        {
            HotKey hotKey = obj as HotKey;
            if (hotKey != null)
                return Equals(hotKey);
            else
                return false;
        }

        public bool Equals(HotKey other)
        {
            return (Key == other.Key && Modifiers == other.Modifiers);
        }

        public override int GetHashCode()
        {
            return (int)Modifiers + 10 * (int)Key;
        }

        public override string ToString()
        {
            return string.Format("{0} + {1} ({2}Enabled)", Key, Modifiers, Enabled ? "" : "Not ");
        }

        /// <summary>
        /// Will be raised if the hotkey is pressed (works only if registed in HotKeyHost)
        /// </summary>
        public event EventHandler<HotKeyEventArgs> HotKeyPressed;

        protected virtual void OnHotKeyPress()
        {
            if (HotKeyPressed != null)
                HotKeyPressed(this, new HotKeyEventArgs(this));
        }

        internal void RaiseOnHotKeyPressed()
        {
            OnHotKeyPress();
        }


        protected HotKey(SerializationInfo info, StreamingContext context)
        {
            Key = (Key)info.GetValue("Key", typeof(Key));
            Modifiers = (ModifierKeys)info.GetValue("Modifiers", typeof(ModifierKeys));
            Enabled = info.GetBoolean("Enabled");
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Key", Key, typeof(Key));
            info.AddValue("Modifiers", Modifiers, typeof(ModifierKeys));
            info.AddValue("Enabled", Enabled);
        }
    }
}
