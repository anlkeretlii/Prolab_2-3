   M S B A M L   `   `   `  ����E  Awpf prolab, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:wpf_prolab.UIwpf_prolab.UI  5   	   (    "wpf_prolab.UI.AddMeasurementWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   -localclr-namespace:wpf_prolab.UI     5   	     � Title$!  Yeni Kan Şekeri Ölçümü��5   	   $	��550��6(   $	��500��65    � WindowStartupLocation$ CenterOwner=�5	   	    � 
ResizeMode$ NoResize=�6-    � 	Resources 5         	FormLabel �� 
TargetType �� Property �� Value  ActionButton��%�  &	        &	 q     �� 5   
   " ��6"   .�5      ��  � ��$ 14(�6)   �� 5       � ��$ 0,8,0,4q�6'   5      �� 5   
   " ��6%   .�5      ��  � �� �#2196F36+   �� 5       � ��
 �����6+   �� 5       � ��$
 15,8q�6(   �� 5       � ��$ 0q�60   �� 5       � ��$ 5q�6'   �� 5       �@�Cursor$
 Hand��6'   5      5      .��5      � $��20q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   �� 5       $
��Auto �6   .+�5$   
   �� $��0��6   $&�� Yeni Kan Şekeri Ölçümü Ekle��6!   $��22(�6D   $
��Bold"�6R   ���#2196F3$��0,0,0,20q�5%   )   �� 5(   
   $��1��6   $
��Auto=�6$   $	��250��6G   .��5)      �� .+�5+      �� $��Tarih ve Saat:��6   #��[  62   � 5,       � ColumnDefinitions 5-      �� 5.      $��* �6+   �� 5/      $��* �6+   .+�52      (  "System.Windows.Controls.DatePicker  -   5        	  Name$	 dpMeasurementDate��52   1   $��0��6!   $��0,0,5,0q�6L   �� 53      -   5        
 ��Name$
 txtMeasurementTime��53   .   $��1��6   $��5,0,0,0q�6J   $��12:00��54      $��Center=�6+   55      �� 58      $��Ölçüm Zamanı:��6   #��[  61   �� 59      -   5         ��Name$ cmbMeasurementType��59       �� SelectedIndex$ 0��67   .@�5:      �� $��Sabah Aç (Morning)��6#   �� 5;      $��Öğle (Noon)��6#   �� 5<      $��İkindi (Afternoon)��6#   �� 5=      $��Akşam (Evening)��6#   �� 5>      $��Gece (Night)��6#   5?      �� 5B      $��Ölçüm Değeri (mg/dL):��6   #��[  69   �� 5C      -   5        $
 txtMeasurementValue��5C      67   �� 5F      -   5         ��Name$ borderStatus��5F      $��30��6/   $��0,10,0,0q�6;   $y�	Collapsed=�6M   ._�5G      �� -   5         ��Name$ 	txtStatus��5G       
�������$��Center=�6F   $��Center=�5H       $��Normal��6;   5I      �� 5L      $!��İnsülin Dozu (Opsiyonel):��6   #��[  6=   �� 5M      -   5        $
 txtInsulinDose��5M      �� 5P      $��Notlar (Opsiyonel):��6   #��[  67   �� 5Q      -   5        $
 txtNotes��5Q      $��80��6,    �� TextWrapping$
 Wrap=�68    �� AcceptsReturn . 6L   5R      5S      �� 5V   
   $��2��6   $��0,15,0,0q�6!   $
��Wrap=�63   ���#555555$��12(�6\   �� 5W      $�Referans değerler:��6   �� 5X      �� 5Y      $.�(• 70 mg/dL altı: Düşük kan şekeri��6   �� 5Z      �� 5[      $(�"• 70 - 110 mg/dL: Normal aralık��6   �� 5\      �� 5]      $.�(• 110 - 180 mg/dL: Yüksek kan şekeri��6   �� 5^      �� 5_      $M�G• 180 mg/dL üstü: Çok yüksek kan şekeri (Acil önlem alınmalı)��6   5`      �� 5c   
   $��3��6    �� Orientation$ 
Horizontal=�6"   $��Right=�6;   $��0,10,0,0q�6W   .+�5d      �� -	   5        $��İptal��5d      $	��100��6%   $��5q�61   6<   �� 5e      -
   5        $��Kaydet��5e      $	��100��6&   #��[ 62   6X   5f      5g      5h      