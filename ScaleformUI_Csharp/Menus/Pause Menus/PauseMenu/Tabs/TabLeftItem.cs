﻿using ScaleformUI.Elements;
using ScaleformUI.Menu;
using ScaleformUI.Scaleforms;

namespace ScaleformUI.PauseMenu
{
    public enum LeftItemType
    {
        Empty,
        Info,
        Statistics,
        Settings,
        Keymap
    }

    public enum LeftItemBGType
    {
        Full,
        Masked,
        Resized
    }

    public delegate void IndexChangeEvent(SettingsItem item, int index);
    public delegate void ActivatedEvent(TabLeftItem item, int index);

    public class TabLeftItem
    {
        internal UIMenuItem _internalItem;
        private bool enabled = true;
        private string label;
        internal string _formatLeftLabel;
        private HudColor mainColor;
        private HudColor highlightColor;
        private string textTitle;
        private string keymapRightLabel_1;
        private string keymapRightLabel_2;
        public string TextureDict { get; private set; }
        public string TextureName { get; private set; }
        public LeftItemBGType LeftItemBGType { get; private set; }

        internal ItemFont _labelFont = ScaleformFonts.CHALET_LONDON_NINETEENSIXTY;
        internal ItemFont _rightLabelFont = ScaleformFonts.CHALET_LONDON_NINETEENSIXTY;
        public LeftItemType ItemType { get; internal set; }
        public string Label
        {
            get => label;
            set
            {
                label = value;
                _formatLeftLabel = value.StartsWith("~") ? value : "~s~" + value;
                if (Selected)
                {
                    _formatLeftLabel = _formatLeftLabel.Replace("~w~", "~l~");
                    _formatLeftLabel = _formatLeftLabel.Replace("~s~", "~l~");
                }
                else
                {
                    _formatLeftLabel = _formatLeftLabel.Replace("~l~", "~s~");
                }
                if (Parent != null && Parent.Visible)
                {
                    int tab = Parent.Parent.Tabs.IndexOf(Parent);
                    int it = Parent.LeftItemList.IndexOf(this);
                    Parent.Parent._pause._pause.CallFunction("UPDATE_LEFT_ITEM_LABEL", tab, it, _formatLeftLabel);
                }
            }
        }
        public HudColor MainColor { get => mainColor; set => mainColor = value; }
        public HudColor HighlightColor { get => highlightColor; set => highlightColor = value; }
        public bool Enabled
        {
            get => enabled;
            set
            {
                enabled = value;
                if (!value)
                    _formatLeftLabel = _formatLeftLabel.ReplaceRstarColorsWith("~c~");
                else
                    Label = label;

                if (Parent != null && Parent.Visible)
                {
                    int tab = Parent.Parent.Tabs.IndexOf(Parent);
                    int it = Parent.LeftItemList.IndexOf(this);
                    Parent.Parent._pause._pause.CallFunction("ENABLE_LEFT_ITEM", tab, it, enabled);
                    Parent.Parent._pause._pause.CallFunction("UPDATE_LEFT_ITEM_LABEL", tab, it, _formatLeftLabel);
                }
            }
        }

        public bool Hovered { get; internal set; }

        public bool Selected
        {
            get => selected;
            internal set
            {
                selected = value;

                if (value)
                {
                    _formatLeftLabel = _formatLeftLabel.Replace("~w~", "~l~");
                    _formatLeftLabel = _formatLeftLabel.Replace("~s~", "~l~");
                }
                else
                {
                    _formatLeftLabel = _formatLeftLabel.Replace("~l~", "~s~");
                }
                if (Parent != null && Parent.Visible)
                {
                    int tab = Parent.Parent.Tabs.IndexOf(Parent);
                    int it = Parent.LeftItemList.IndexOf(this);
                    Parent.Parent._pause._pause.CallFunction("UPDATE_LEFT_ITEM_LABEL", tab, it, _formatLeftLabel);
                }
            }
        }

        public int ItemIndex { get; internal set; }
        public List<BasicTabItem> ItemList = new List<BasicTabItem>();
        private bool selected;

        public string RightTitle
        {
            get => textTitle;
            set
            {
                textTitle = value;
                if (Parent != null && Parent.Visible)
                {
                    int tab = Parent.Parent.Tabs.IndexOf(Parent);
                    int it = Parent.LeftItemList.IndexOf(this);
                    Parent.Parent._pause._pause.CallFunction("ADD_RIGHT_TITLE", tab, it, textTitle, KeymapRightLabel_1, KeymapRightLabel_2);
                }
            }
        }

        public string KeymapRightLabel_1
        {
            get => keymapRightLabel_1;
            set
            {
                keymapRightLabel_1 = value;
                if (Parent != null && Parent.Visible)
                {
                    int tab = Parent.Parent.Tabs.IndexOf(Parent);
                    int it = Parent.LeftItemList.IndexOf(this);
                    Parent.Parent._pause._pause.CallFunction("ADD_RIGHT_TITLE", tab, it, textTitle, keymapRightLabel_1, KeymapRightLabel_2);
                }
            }
        }
        public string KeymapRightLabel_2
        {
            get => keymapRightLabel_2;
            set
            {
                keymapRightLabel_2 = value;
                if (Parent != null && Parent.Visible)
                {
                    int tab = Parent.Parent.Tabs.IndexOf(Parent);
                    int it = Parent.LeftItemList.IndexOf(this);
                    Parent.Parent._pause._pause.CallFunction("ADD_RIGHT_TITLE", tab, it, textTitle, KeymapRightLabel_1, keymapRightLabel_2);
                }
            }
        }

        public event IndexChangeEvent OnIndexChanged;
        public event ActivatedEvent OnActivated;
        public BaseTab Parent { get; set; }

        public TabLeftItem(string label, LeftItemType type, HudColor mainColor = HudColor.NONE, HudColor highlightColor = HudColor.NONE)
        {
            Label = label;
            ItemType = type;
            MainColor = mainColor;
            HighlightColor = highlightColor;
        }
        public TabLeftItem(string label, LeftItemType type, ItemFont labelFont, HudColor mainColor = HudColor.NONE, HudColor highlightColor = HudColor.NONE)
        {
            Label = label;
            ItemType = type;
            MainColor = mainColor;
            HighlightColor = highlightColor;
            _labelFont = labelFont;
        }

        /// <summary>
        /// Image suggested not bigger than 720 x 576
        /// </summary>
        /// <param name="txd"></param>
        /// <param name="txn"></param>
        public void UpdateBackground(string txd, string txn, LeftItemBGType resizeType)
        {
            TextureDict = txd;
            TextureName = txn;
            LeftItemBGType = resizeType;
            if (Parent != null && Parent.Visible && Parent.Parent != null && Parent.Parent.Visible)
            {
                Parent.Parent._pause._pause.CallFunction("UPDATE_LEFT_ITEM_RIGHT_BACKGROUND", Parent.Parent.Tabs.IndexOf(Parent), Parent.LeftItemList.IndexOf(this), txd, txn, (int)resizeType);
            }
        }

        public void AddItem(BasicTabItem item)
        {
            item.Parent = this;
            ItemList.Add(item);
        }

        internal void IndexChanged()
        {
            OnIndexChanged?.Invoke(ItemList[ItemIndex] as SettingsItem, ItemIndex);
        }

        internal void Activated()
        {
            OnActivated?.Invoke(this, Parent.LeftItemList.IndexOf(this));
        }
    }
}
