using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfApiWeatherNews
{

    [Serializable]
    public  class CustomHotKey : HotKey
    {
        private Action<Key, ModifierKeys> _onHotKeyPressHandler;
        public CustomHotKey(Key key, ModifierKeys modifiers, Action<Key, ModifierKeys> onHotKeyPressAction = null)
            : base(key, modifiers, true)
        {
            _onHotKeyPressHandler = onHotKeyPressAction;
        }
        
        protected override void OnHotKeyPress()
        {
            _onHotKeyPressHandler?.Invoke(Key, Modifiers);
            base.OnHotKeyPress();
        }
    }

}
