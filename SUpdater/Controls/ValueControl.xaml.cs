using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SUpdater.Model;
using SUpdater.Provider;
using ValueType = SUpdater.Model.ValueType;

namespace SUpdater.Controls
{
    /// <summary>
    /// Interaktionslogik für ValueControl.xaml
    /// </summary>
    public partial class ValueControl : UserControl
    {
        public ValueControl()
        {
            InitializeComponent();
        }


        static ValueControl()
        {
            EditModeProperty.OverrideMetadata(typeof(ValueControl), new FrameworkPropertyMetadata(EditModeChanged, EditModeChanging));
        }

        public static readonly DependencyProperty EditModeProperty = DependencyProperty.RegisterAttached(
            "EditMode", typeof(bool), typeof(ValueControl), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));

        public static void SetEditMode(DependencyObject element, bool value)
        {
            element.SetValue(EditModeProperty, value);
        }

        public static bool GetEditMode(DependencyObject element)
        {
            return (bool)element.GetValue(EditModeProperty);
        }

        private static void EditModeChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var valueControl = dependencyObject as ValueControl;
            if (valueControl != null)
                valueControl.Combo.Visibility = (bool)dependencyPropertyChangedEventArgs.NewValue ? Visibility.Visible : Visibility.Collapsed;
        }

        private static object EditModeChanging(DependencyObject dependencyObject, object baseValue)
        {
            var that = dependencyObject as ValueControl;
            Debug.Assert(that != null, "that != null");

            that.Combo.Visibility = Visibility.Collapsed;
            that.Text.Visibility = Visibility.Collapsed;
            that.LongText.Visibility = Visibility.Collapsed;
            that.Image.Visibility = Visibility.Collapsed;
            that.None.Visibility = Visibility.Collapsed;

            var editMode = (bool)baseValue;
            if (editMode)
            {
                that.Combo.Visibility = Visibility.Visible;
            }
            else
            {
                if (that.Value == null)
                {
                    that.None.Visibility = Visibility.Visible;
                }
                else
                { 
                    switch (that.Value.Definition.Type)
                    {
                        case ValueType.Image:
                            Binding b = new Binding();
                            b.Source = that;
                            b.Path = new PropertyPath("Value.ImageData.ImageSource");
                            BindingOperations.SetBinding(that.Image, Image.SourceProperty, b);
                            that.Image.Visibility = Visibility.Visible;
                            break;
                        case ValueType.LongString:
                            BindingOperations.ClearBinding(that.Image, Image.SourceProperty);
                            that.LongText.Visibility = Visibility.Visible;
                            break;
                        default:
                            BindingOperations.ClearBinding(that.Image, Image.SourceProperty);
                            that.Text.Visibility = Visibility.Visible;
                            break;
                    }
                }
            }
            return baseValue;
        }


        readonly ObservableCollection<ValueDefinition> _definitions = new ObservableCollection<ValueDefinition>();

        public ReadOnlyObservableCollection<ValueDefinition> Definitions => new ReadOnlyObservableCollection<ValueDefinition>(_definitions);


        public static readonly DependencyPropertyKey ValuePropertyKey = DependencyProperty.RegisterReadOnly(
            "Value", typeof (Value), typeof (ValueControl), new PropertyMetadata(default(Value)));

        public static readonly DependencyProperty ValueProperty = ValuePropertyKey.DependencyProperty;

        public Value Value
        {
            get { return (Value) GetValue(ValueProperty); }
            set { SetValue(ValuePropertyKey, value); }
        }
        

        public static readonly DependencyProperty ValuePathProperty = DependencyProperty.Register(
            "ValuePath", typeof (String), typeof (ValueControl), new PropertyMetadata(default(String),ValuePathChanged));

        private static void ValuePathChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var that = dependencyObject as ValueControl;
            Debug.Assert(that != null, "that != null");
            that.UpdateValue();
        }

        public String ValuePath
        {
            get { return (String) GetValue(ValuePathProperty); }
            set { SetValue(ValuePathProperty, value); }
        }

        void UpdateValue()
        {
            if (Entity != null && ValuePath!=null)
            {
                Value = Entity[ValuePath];
            }
        }


        public static readonly DependencyProperty EntityProperty = DependencyProperty.Register(
            "Entity", typeof (Entity), typeof (ValueControl), new PropertyMetadata(default(Entity),EntityPropChanged, EntityPropertyChanging));

        private static void EntityPropChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var entity = dependencyPropertyChangedEventArgs.NewValue as Entity;
            if (entity != null)
            {
                var that = dependencyObject as ValueControl;
                Debug.Assert(that != null, "that != null");

                var oldEntity = dependencyPropertyChangedEventArgs.OldValue as Entity;
                if (oldEntity == null || oldEntity.Type != entity.Type)
                {
                    that.UpdatePossibilites(entity.Type);
                }
                that.UpdateValue();
            }
        }

        private void UpdatePossibilites(EntityType type)
        {
            _definitions.Clear();
            foreach (IProvider prov in ProviderManager.GetProviders())
            {
                prov.Values.Where(v => v.EntityType == type).ToList().ForEach(vd => _definitions.Add(vd));
            }
        }

        private static object EntityPropertyChanging(DependencyObject dependencyObject, object baseValue)
        {
            if (baseValue == null) return null; //unsetting is allowed

            var that = dependencyObject as ValueControl;
            Debug.Assert(that != null, "that != null");

            if (that.EntityType.HasValue)
            {
                throw new InvalidOperationException("Cannot set both Entity and EntityType");
                //return DependencyProperty.UnsetValue;
            }
            return baseValue;
        }

        public Entity Entity
        {
            get { return (Entity) GetValue(EntityProperty); }
            set { SetValue(EntityProperty, value); }
        }

        public static readonly DependencyProperty EntityTypeProperty = DependencyProperty.Register(
            "EntityType", typeof(EntityType?), typeof (ValueControl), new PropertyMetadata(default(EntityType?), EntityTypeChanged, EntityTypeChanging));

        private static void EntityTypeChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var entityType = dependencyPropertyChangedEventArgs.NewValue as EntityType?;
            if (entityType.HasValue)
            {
                var that = dependencyObject as ValueControl;
                Debug.Assert(that != null, "that != null");
                that.UpdatePossibilites(entityType.Value);

            }
        }

        private static object EntityTypeChanging(DependencyObject dependencyObject, object baseValue)
        {
            if (baseValue == null) return null; //unsetting is allowed

            var that = dependencyObject as ValueControl;
            Debug.Assert(that != null, "that != null");

            if (that.Entity!=null)
            {
                throw new InvalidOperationException("Cannot set both Entity and EntityType");
                //return DependencyProperty.UnsetValue;
            }
            return baseValue;
        }

        public EntityType? EntityType
        {
            get { return (EntityType?) GetValue(EntityTypeProperty); }
            set { SetValue(EntityTypeProperty, value); }
        }

    }
}
