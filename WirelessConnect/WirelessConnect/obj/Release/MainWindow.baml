   M S B A M L   `   `   `  ���� [  WSystem.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089Y@clr-namespace:System.Windows.Forms;assembly=System.Windows.FormsSystem.Windows.Forms  J FWirelessConnect, Version=1.1.5.0, Culture=neutral, PublicKeyToken=null1clr-namespace:WirelessConnectWirelessConnect     WirelessConnect.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     RForms@clr-namespace:System.Windows.Forms;assembly=System.Windows.Forms     /localclr-namespace:WirelessConnect       � Title$  Wireless Connect��$	��768��$
��1152�� � WindowStartupLocation$ CenterScreen=� � WindowStyle$
 None=� � DataContext %  WirelessConnect.ConnectionState  .��� $��0,0,0,0q� � ColumnDefinitions �� �� $��0* �.+��� -    ��Name$ image��$
��Left=�$��25��$��5,1,0,0q�$	��Top=�$��25��$��w-lan-icon-1.ico���� -    ��Name$ label_Header��$��Wireless Connect��$
��Left=�$	��Top=�$��14(�$��25,1,0,0q��� $��65��$��0,1,0,0q�$	��Top=��� -   $ label_Networks��$��Choose Wireless Network:��$
��Left=�$��	40,53,0,0q�$	��Top=�$��28(�$��44���� -    ��Name$ listBox_Networks��$��40,114,473,151.333q�$��28(��� -   $ label_SecurityKey��$��Security Key:��$��
0,53,226,0q�$	��Top=�$��28(�$��Right=�$	��213��$��44���� -    ��Name$ textBox_SecurityKey��$��40��$��
0,114,40,0q�	 �� TextWrapping$
	 Wrap=�$	��Top=�$��28(�$��Right=�$	��399���� -   $ label_SecurityKey_Clear��$��X��$��0,112,44.333,0q�$	��Top=�$��24(�
 K� ZIndex$
 1��$��Right=�$��26���� -    ��Name$ button_Refresh��$��40,0,473.333,82.333q�$��Bottom=�$��53��$��28(����	#FFF0F0F0$"��Initializing, please wait...���� -	   $ label_ManualConfiguration��$ ��      Manual Configuration��$��0,166,66.333,0q�$	��Top=�$��24(� � Cursor$
 Hand��$��Right=�$	��373��$
 1���� -
   $ label_NetworkName��$��Network Name:��$��0,226,226,0q�$	��Top=�$��24(�$��Right=�$	��213��$��44��$y�Hidden=��� -   $ textBox_NetworkName��$��40��$��
0,270,40,0q�$
	 Wrap=�$	��Top=�$��28(�$��Right=�$	��399��$y�Hidden=��� -   $ label_NetworkName_Clear��$��X��$��0,268,44.333,0q�$	��Top=�$��24(�$
 1��$��Right=�$��26��$y�	Collapsed=��� -   $ label_Encryption��$��Authentication:��$��0,328,210,0q�$	��Top=�$��24(�$��Right=�$	��229��$��44��$y�Hidden=��� -    ��Name$ comboBox_Authentication��$��0,377,43.333,0q�$	��Top=�$��44��$��Right=�$��28(� �� SelectedIndex$ 0��$y�Hidden=�$	��396������  �� EndPoint$	 0,1&� �� 
StartPoint$	 0,0&�.���  � Color$ 	#FFFFFFFF�� � Offset$ 0Q�� $ White��$ 1Q�.@��� $
��WPA2���� $	��WPA���� $	��WEP���� $
��Open���� -   $ 
button_OSK��$��Keyboard��$��0,0,40.333,219.333q�$��53��$��Bottom=�$��28(�$��Right=�$	��399�����	#FFF0F0F0�� -   $ button_ConnectDisconnect��$��Connect��$��0,0,40.333,151.333q�$��28(�$��53��$��Bottom=�$��Right=�$	��399�����	#FFF0F0F0�� -   $ button_Continue��$��Continue��$��0,0,40.333,82.333q�$��53��$��Bottom=�$��28(�$��Right=�$	��399��{�.  ���� *InternetAccessButtonColor+ �� Mode$ OneWay=��� -   $ label_Wired��$��Wired Network��$��341,0,0,28.333q�$��39��$��Bottom=�$��24(�$
��Left=�$	��192���� -    ��Name$ Canvas_Wired��$��
543,0,0,36q�$��17��$��Bottom=�$
��Left=�$��18������ *Wired+$ OneWay=��� -   $ label_Wireless��$��Wireless Network��$��59,0,0,28.333q�$��39��$��Bottom=�$��24(�$
��Left=�$	��232���� -   $ Canvas_Wireless��$��
292,0,0,36q�$��17��$��Bottom=�$
��Left=�$��18������ *
Wireless+$ OneWay=��� -   $ label_InternetAccess��$��Internet Access��$��0,0,73,28.333q�$��Right=�$	��201��$��39��$��Bottom=�$��24(��� -   $ Canvas_InternetAccess��$��	0,0,54,36q�$��Right=�$��18��$��17��$��Bottom=����� *InternetAccess+$ OneWay=�9� -    9�Name$" expander_ManualConfiguration��$�� ��$��Right=�$��0,177,406.333,0q�$	��Top=�$
 -1��.��� ���	#FFE5E5E5$
 Hand��7  1System.Windows.Forms.Integration.WindowsFormsHost  -     Name$ winFormHost��$��327,336,0,0q�$��45��$	��Top=�$
��Left=�$��45��   Child. %   System.Windows.Forms.PictureBox  -   pictureBox_Initializing��"   System.Windows.Forms.Control   Width$ 45��   Height$ 45��