   M S B A M L   `   `   `  ����[  WSystem.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089Y@clr-namespace:System.Windows.Forms;assembly=System.Windows.FormsSystem.Windows.Forms  5   	   J FWirelessConnect, Version=1.1.5.0, Culture=neutral, PublicKeyToken=null1clr-namespace:WirelessConnectWirelessConnect 5   	       WirelessConnect.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   RForms@clr-namespace:System.Windows.Forms;assembly=System.Windows.Forms     5   	   /localclr-namespace:WirelessConnect     5   	     � Title$  Wireless Connect��5
   	   $	��768��6"   $
��1152��6/    � WindowStartupLocation$ CenterScreen=�6<    � WindowStyle$
 None=�6a    � DataContext 5      %  WirelessConnect.ConnectionState  5   
   .��5      � $��0,0,0,0q�6    � ColumnDefinitions 5   
   �� 5      �� 5      $��0* �6   .+�5   
   �� -   5         ��Name$ image��5      $
��Left=�6   $��25��6:   $��5,1,0,0q�6F   $	��Top=�6W   $��25��6o   $��w-lan-icon-1.ico��6z   �� 5   
   -   5         ��Name$ label_Header��5      $��Wireless Connect��6&   $
��Left=�6A   $	��Top=�6\   $��14(�6t   $��25,1,0,0q�6�   �� 5   
   $��65��6   $��0,1,0,0q�6    $	��Top=�61   �� 5   
   -   5        $ label_Networks��5      $��Choose Wireless Network:��6(   $
��Left=�6K   $��	40,53,0,0q�6f   $	��Top=�6y   $��28(�6�   $��44��6�   �� 5   
   -   5         ��Name$ listBox_Networks��5      $��40,114,473,151.333q�6,   $��28(�6H   6V   �� 5   
   -   5        $ label_SecurityKey��5      $��Security Key:��6+   $��
0,53,226,0q�6C   $	��Top=�6W   $��28(�6o   $��Right=�6}   $	��213��6�   $��44��6�   �� 5   
   -   5         ��Name$ textBox_SecurityKey��5      $��40��6/   $��
0,114,40,0q�6;   	 �� TextWrapping$
	 Wrap=�6O   $	��Top=�6c   $��28(�6{   $��Right=�6�   $	��399��6�   6�   �� 5    
   -   5        $ label_SecurityKey_Clear��5       $��X��61   $��0,112,44.333,0q�6=   $	��Top=�6U   $��24(�6m   
 K� ZIndex$
 1��6{   $��Right=�6�   $��26��6�   6�   �� 5"   
   -   5         ��Name$ button_Refresh��5"      $��40,0,473.333,82.333q�6)   $��Bottom=�6F   $��53��6~   $��28(�6�   ���	#FFF0F0F0$"��Initializing, please wait...��6�   �� 5$   
   -	   5        $ label_ManualConfiguration��5$      $ ��      Manual Configuration��63   $��0,166,66.333,0q�6X   $	��Top=�6p   $��24(�6�    � Cursor$
 Hand��6�   $��Right=�6  $	��373��6  $
 1��6)  �� 5&   
   -
   5        $ label_NetworkName��5&      $��Network Name:��6+   $��0,226,226,0q�6C   $	��Top=�6X   $��24(�6p   $��Right=�6~   $	��213��6�   $��44��6�   $y�Hidden=�6�   �� 5'   
   -   5        $ textBox_NetworkName��5'      $��40��6/   $��
0,270,40,0q�6;   $
	 Wrap=�6O   $	��Top=�6c   $��28(�6{   $��Right=�6�   $	��399��6�   $y�Hidden=�6�   6�   �� 5(   
   -   5        $ label_NetworkName_Clear��5(      $��X��61   $��0,268,44.333,0q�6=   $	��Top=�6U   $��24(�6m   $
 1��6{   $��Right=�6�   $��26��6�   $y�	Collapsed=�6  �� 5*   
   -   5        $ label_Encryption��5*      $��Authentication:��6*   $��0,328,210,0q�6D   $	��Top=�6Y   $��24(�6q   $��Right=�6   $	��229��6�   $��44��6�   $y�Hidden=�6�   �� 5+   
   -   5         ��Name$ comboBox_Authentication��5+      $��0,377,43.333,0q�64   $	��Top=�6L   $��44��6d   $��Right=�6p   $��28(�6�    �� SelectedIndex$ 0��6�   $y�Hidden=�6�   $	��396��6�   ��5,      �� 5-       �� EndPoint$	 0,1&�6&    �� 
StartPoint$	 0,0&�65   .��5.      �  � Color$ 	#FFFFFFFF��6#    � Offset$ 0Q�65   � 5/      $ White��6#   $ 1Q�61   50      .@�52      �� $
��WPA2��6   �� 53      $	��WPA��6   �� 54      $	��WEP��6   �� 55      $
��Open��6   56      �� 58   
   -   5        $ 
button_OSK��58      $��Keyboard��6%   $��0,0,40.333,219.333q�68   $��53��6T   $��Bottom=�6`   $��28(�6{   $��Right=�6�   $	��399��6�   ���	#FFF0F0F06�   �� 59   
   -   5        $ button_ConnectDisconnect��59      $��Connect��63   $��0,0,40.333,151.333q�6E   $��28(�6�   $��53��6�   $��Bottom=�6�   $��Right=�6�   $	��399��6�   ���	#FFF0F0F06�   �� 5:   
   -   5        $ button_Continue��5:      $��Continue��6*   $��0,0,40.333,82.333q�6=   $��53��6v   $��Bottom=�6�   $��28(�6�   $��Right=�6�   $	��399��6�   {�.  ��6�   �� *InternetAccessButtonColor+ �� Mode$ OneWay=��� 5<   
   -   5        $ label_Wired��5<      $��Wired Network��6%   $��341,0,0,28.333q�6=   $��39��6U   $��Bottom=�6a   $��24(�6|   $
��Left=�6�   $	��192��6�   �� 5=   
   -   5         ��Name$ Canvas_Wired��5=      $��
543,0,0,36q�6'   $��17��6e   $��Bottom=�6q   $
��Left=�6�   $��18��6�   ��6;   �� *Wired+$ OneWay=�6�   �� 5?   
   -   5        $ label_Wireless��5?      $��Wireless Network��6(   $��59,0,0,28.333q�6C   $��39��6Z   $��Bottom=�6f   $��24(�6�   $
��Left=�6�   $	��232��6�   �� 5@   
   -   5        $ Canvas_Wireless��5@      $��
292,0,0,36q�6*   $��17��6k   $��Bottom=�6w   $
��Left=�6�   $��18��6�   ��6>   �� *
Wireless+$ OneWay=�6�   �� 5B   
   -   5        $ label_InternetAccess��5B      $��Internet Access��6.   $��0,0,73,28.333q�6H   $��Right=�6_   $	��201��6{   $��39��6�   $��Bottom=�6�   $��24(�6�   �� 5C   
   -   5        $ Canvas_InternetAccess��5C      $��	0,0,54,36q�60   $��Right=�6v   $��18��6�   $��17��6�   $��Bottom=�6�   ��6C   �� *InternetAccess+$ OneWay=�6�   9� 5E   
   -   5         9�Name$" expander_ManualConfiguration��5E      $�� ��69   $��Right=�6C   $��0,177,406.333,0q�6_   $	��Top=�6x   $
 -1��6�   .��5F      � ���	#FFE5E5E5$
 Hand��6*   5G      7  1System.Windows.Forms.Integration.WindowsFormsHost  5H   
   -   5          Name$ winFormHost��5H      $��327,336,0,0q�60   $��45��6E   $	��Top=�6Q   $
��Left=�6i   $��45��6�      Child. 5I      %   System.Windows.Forms.PictureBox  -   5        pictureBox_Initializing��"   System.Windows.Forms.Control   Width$ 45��5I   @      Height$ 45��6K   5J      5K      5L      