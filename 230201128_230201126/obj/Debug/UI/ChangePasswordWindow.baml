   M S B A M L   `   `   `  ����  
wpf prolab(    "wpf_prolab.UI.ChangePasswordWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  Şifre Değiştir��5   	   $	��350��6    $	��450��6-    � WindowStartupLocation$ CenterScreen=�5   	    � 
ResizeMode$ NoResize=�6.   .��5      � $��20q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5	      $
��Auto �6   �� 5
      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� $��0��6   $��🔐 Şifre Değiştir��6!   $��20(�6:   $
��Bold"�6H   $��Center=�5      ���#2c5aa0$��0,0,0,20q�6F   �� 5   
   $��1��6   $6��0Güvenliğiniz için yeni bir şifre belirleyin:��6!   $��Center=�5      $��0,0,0,20q�61   $
��Wrap=�6C   �� 5   
   $��2��6   $��Mevcut Şifre:��6!   $
��Bold"�66   $	��0,5q�6H   G� 5   
   -   5         G�Name$ txtCurrentPassword��5   #   $��3��6   $��0,5,0,15q�6?   $��30��5      6"   �� 5!   
   $��4��6   $��Yeni Şifre:��6!   $
��Bold"�64   $	��0,5q�6F   G� 5"   
   -   5        $ txtNewPassword��5"   #   $��5��6   $��0,5,0,5q�6;   $��30��5#      6"   �� 5&   
   $��6��6   $��Yeni Şifre (Tekrar):��6!   $
��Bold"�6=   $	��0,5q�6O   G� 5'   
   -   5        $ txtConfirmPassword��5'   #   $��7��6   $��0,5,0,15q�6?   $��30��5(      6"   �� 5+   
   -   5         ��Name$ txtPasswordError��5+      $��7��6.   $��Şifreler eşleşmiyor!��6;   
���  ��$y�	Collapsed=�5,   %   $��0,35,0,0q�6<   �� 5/   
   $��8��6    �� Orientation$ 
Horizontal=�6"   $��Center=�6;   $��0,20,0,0q�6X   .+�50      �� -   5         ��Name$ 	btnCancel��50      $��İptal��6(   $	��100��68   $��35��6D   $��5q�6P   ���#6c757d
�������51   B   �� 52      -   5        $ btnSave��52      $��Kaydet��6&   $	��100��67   $��35��6C   $��5q�6O   ���#2c5aa0
�������{�.  53   S   54      55      56      