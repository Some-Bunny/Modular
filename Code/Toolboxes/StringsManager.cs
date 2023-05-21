using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ModularMod
{
    public class AdvancedStringDB
    {
        public StringTableManager.GungeonSupportedLanguages CurrentLanguage
        {
            get
            {
                return GameManager.Options.CurrentLanguage;
            }
            set
            {
                StringTableManager.SetNewLanguage(value, true);
            }
        }

        public AdvancedStringDB()
        {
            StringDB strings = ETGMod.Databases.Strings;
            strings.OnLanguageChanged = (Action<StringTableManager.GungeonSupportedLanguages>)Delegate.Combine(strings.OnLanguageChanged, new Action<StringTableManager.GungeonSupportedLanguages>(this.LanguageChanged));
            this.Core = new AdvancedStringDBTable(() => StringTableManager.CoreTable);
            this.Items = new AdvancedStringDBTable(() => StringTableManager.ItemTable);
            this.Enemies = new AdvancedStringDBTable(() => StringTableManager.EnemyTable);
            this.Intro = new AdvancedStringDBTable(() => StringTableManager.IntroTable);
            this.Synergies = new AdvancedStringDBTable(() => AdvancedStringDB.SynergyTable);
        }

        // Token: 0x060000ED RID: 237 RVA: 0x00009830 File Offset: 0x00007A30
        public void LanguageChanged(StringTableManager.GungeonSupportedLanguages newLang)
        {
            this.Core.LanguageChanged();
            this.Items.LanguageChanged();
            this.Enemies.LanguageChanged();
            this.Intro.LanguageChanged();
            this.Synergies.LanguageChanged();
            Action<StringTableManager.GungeonSupportedLanguages> onLanguageChanged = this.OnLanguageChanged;
            bool flag = onLanguageChanged == null;
            if (!flag)
            {
                onLanguageChanged(newLang);
            }
        }

        public static Dictionary<string, StringTableManager.StringCollection> SynergyTable
        {
            get
            {
                StringTableManager.GetSynergyString("ThisExistsOnlyToLoadTables", -1);
                return (Dictionary<string, StringTableManager.StringCollection>)AdvancedStringDB.m_synergyTable.GetValue(null);
            }
        }

        public readonly AdvancedStringDBTable Core;

        public readonly AdvancedStringDBTable Items;

        public readonly AdvancedStringDBTable Enemies;

        public readonly AdvancedStringDBTable Intro;

        public readonly AdvancedStringDBTable Synergies;

        public static FieldInfo m_synergyTable = typeof(StringTableManager).GetField("m_synergyTable", BindingFlags.Static | BindingFlags.NonPublic);

        public Action<StringTableManager.GungeonSupportedLanguages> OnLanguageChanged;
    }
    public sealed class AdvancedStringDBTable
    {
        public Dictionary<string, StringTableManager.StringCollection> Table
        {
            get
            {
                Dictionary<string, StringTableManager.StringCollection> result;
                bool flag = (result = this._CachedTable) == null;
                if (flag)
                {
                    result = (this._CachedTable = this._GetTable());
                }
                return result;
            }
        }

        public StringTableManager.StringCollection this[string key]
        {
            get
            {
                return this.Table[key];
            }
            set
            {
                this.Table[key] = value;
                int num = this._ChangeKeys.IndexOf(key);
                bool flag = num > 0;
                if (flag)
                {
                    this._ChangeValues[num] = value;
                }
                else
                {
                    this._ChangeKeys.Add(key);
                    this._ChangeValues.Add(value);
                }
                JournalEntry.ReloadDataSemaphore++;
            }
        }

        internal AdvancedStringDBTable(Func<Dictionary<string, StringTableManager.StringCollection>> _getTable)
        {
            this._ChangeKeys = new List<string>();
            this._ChangeValues = new List<StringTableManager.StringCollection>();
            this._GetTable = _getTable;
        }

        public bool ContainsKey(string key)
        {
            return this.Table.ContainsKey(key);
        }

        public void Set(string key, string value)
        {
            StringTableManager.StringCollection stringCollection = new StringTableManager.SimpleStringCollection();
            stringCollection.AddString(value, 1f);
            bool flag = this.Table.ContainsKey(key);
            if (flag)
            {
                this.Table[key] = stringCollection;
            }
            else
            {
                this.Table.Add(key, stringCollection);
            }
            int num = this._ChangeKeys.IndexOf(key);
            bool flag2 = num > 0;
            if (flag2)
            {
                this._ChangeValues[num] = stringCollection;
            }
            else
            {
                this._ChangeKeys.Add(key);
                this._ChangeValues.Add(stringCollection);
            }
            JournalEntry.ReloadDataSemaphore++;
        }

        public void SetComplex(string key, List<string> values, List<float> weights)
        {
            StringTableManager.StringCollection stringCollection = new StringTableManager.ComplexStringCollection();
            for (int i = 0; i < values.Count; i++)
            {
                string text = values[i];
                float num = weights[i];
                stringCollection.AddString(text, num);
            }
            this.Table[key] = stringCollection;
            int num2 = this._ChangeKeys.IndexOf(key);
            bool flag = num2 > 0;
            if (flag)
            {
                this._ChangeValues[num2] = stringCollection;
            }
            else
            {
                this._ChangeKeys.Add(key);
                this._ChangeValues.Add(stringCollection);
            }
            JournalEntry.ReloadDataSemaphore++;
        }

        public string Get(string key)
        {
            return StringTableManager.GetString(key);
        }

        public void LanguageChanged()
        {
            this._CachedTable = null;
            Dictionary<string, StringTableManager.StringCollection> table = this.Table;
            for (int i = 0; i < this._ChangeKeys.Count; i++)
            {
                table[this._ChangeKeys[i]] = this._ChangeValues[i];
            }
        }

        private readonly Func<Dictionary<string, StringTableManager.StringCollection>> _GetTable;

        private Dictionary<string, StringTableManager.StringCollection> _CachedTable;

        private readonly List<string> _ChangeKeys;

        private readonly List<StringTableManager.StringCollection> _ChangeValues;
    }
}
