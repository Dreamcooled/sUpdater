using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SUpdater.Model;

namespace SUpdater.Controls
{
    /// <summary>
    /// Interaktionslogik für ValueControlAdder.xaml
    /// </summary>
    public partial class ValueControlAdder : UserControl
    {
        public ValueControlAdder()
        {
            InitializeComponent();
            Visibility = (bool) GetValue(ValueControl.EditModeProperty) ? Visibility.Visible : Visibility.Collapsed;
            var description = DependencyPropertyDescriptor.FromProperty(ValueControl.EditModeProperty,typeof(ValueControl));
            description.AddValueChanged(this,EditModeChanged);
        }

        private void EditModeChanged(object sender, EventArgs eventArgs)
        {
            Visibility = (bool)GetValue(ValueControl.EditModeProperty) ? Visibility.Visible : Visibility.Collapsed;
        }

        public static readonly DependencyProperty EditModeProperty = DependencyProperty.Register(
            "EditMode", typeof (bool?), typeof (ValueControlAdder), new PropertyMetadata(null));

        public bool? EditMode
        {
            get { return (bool?) GetValue(EditModeProperty); }
            set { SetValue(EditModeProperty, value); }
        }

        public static readonly DependencyProperty EntityProperty = DependencyProperty.Register(
            "Entity", typeof (Entity), typeof (ValueControlAdder), new PropertyMetadata(default(Entity)));


        public Entity Entity
        {
            get { return (Entity) GetValue(EntityProperty); }
            set { SetValue(EntityProperty, value); }
        }

        public static readonly DependencyProperty ValuePathProperty = DependencyProperty.Register(
            "ValuePath", typeof (string), typeof (ValueControlAdder), new PropertyMetadata(default(string)));

        public string ValuePath
        {
            get { return (string) GetValue(ValuePathProperty); }
            set { SetValue(ValuePathProperty, value); }
        }

        public static readonly DependencyProperty EntityTypeProperty = DependencyProperty.Register(
            "EntityType", typeof (EntityType?), typeof (ValueControlAdder), new PropertyMetadata(null));

        public EntityType? EntityType
        {
            get { return (EntityType?) GetValue(EntityTypeProperty); }
            set { SetValue(EntityTypeProperty, value); }
        }


        private void AddButtonClicked(object sender, RoutedEventArgs e)
        {
            var parent = VisualTreeHelper.GetParent(this) as Panel;
            if (parent != null)
            {
                int myInd = parent.Children.IndexOf(this);

                var newControl = new ValueControl();
                newControl.Margin = this.Margin;
                newControl.HorizontalAlignment = this.HorizontalAlignment;
                newControl.VerticalAlignment = this.VerticalAlignment;

                if(EditMode.HasValue) newControl.SetValue(ValueControl.EditModeProperty,EditMode.Value);
                if (Entity != null) newControl.Entity = Entity;
                if (ValuePath != null) newControl.ValuePath = ValuePath;
                if (EntityType.HasValue) newControl.EntityType = EntityType.Value;

                parent.Children.Insert(myInd,newControl);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }
}
