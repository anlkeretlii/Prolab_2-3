   M S B A M L   `   `   `  ����E  Awpf prolab, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:wpf_prolab.UIwpf_prolab.UI  5   	   $    wpf_prolab.UI.AddPatientWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   -localclr-namespace:wpf_prolab.UI     5   	     � Title$  Yeni Hasta Ekle��5   	   $	��600��6!   $	��500��6.    � WindowStartupLocation$ CenterScreen=�5	   	    � 
ResizeMode$ NoResize=�5
   	    � 	Resources 5         	FormLabel �� 
TargetType �� Property �� Value  	FormInput  
FormButton��%�  &	        &	 q     &	     �� 5   
   " ��6"   .�5      ��  � ��$ 14(�6)   �� 5       � ��$ 0,8,0,4q�6'   5      �� 5   
   " ��6"   .�5      ��  � ��$ 32��6'   �� 5       � ��$	 8,0q�6(   �� 5       � ��$ 0,0,0,8q�6'   5      �� 5   
   " ��6#   .�5      ��  � ��$ 40��6'   �� 5       � ��$
 20,0q�6(   �� 5       � ��$ 8q�6'   5      5       .��5"      � $��20q�6    � RowDefinitions 5#   
   �� 5$      $
��Auto �6   �� 5%      $��* �6   �� 5&      $
��Auto �6   .+�5*   
   �� $��0��6   $��Yeni Hasta Kaydı��6!   $��22(�69   $
��Bold"�6G   ���#2196F3$��0,0,0,20q�6n   �� 5-   
   $��1��6   $
��Auto=�6$   .��5.      �� .+�50      �� $��TC Kimlik No:��6   #��[  61   �� 51      -   5         ��Name$ txtTcId��51      #��[ 6+   	 �� 	MaxLength$	 11��6N   6]   �� 52      -   5        
 ��Name$
 txtTcIdError��52      
���  ��$y�	Collapsed=�6C   $)��#TC Kimlik No 11 haneli olmalıdır.��6Z   $��12(�6�   $��0,-4,0,8q�6�   �� 55      $	��Ad:��6   #��[  6'   �� 56      -   5        $ txtFirstName��56      #��[ 60   6S   �� 59      $��Soyad:��6   #��[  6*   �� 5:      -   5        $ txtLastName��5:      #��[ 6/   6R   �� 5=      $��Doğum Tarihi:��6   #��[  61   (  "System.Windows.Controls.DatePicker  5>      -   5          Name$ dpBirthDate��5>      #��[ 62   6U   �� 5A      $��	Cinsiyet:��6   #��[  6-   �� 5B       �� Orientation$ 
Horizontal=�6   $��0,0,0,8q�66   .+�5C      	� -   5         	�Name$ rbMale��5C   "   $��Erkek��62   $��0,0,20,0q�6B    d� 	IsChecked$
 TrueY�6r   	� 5D      -   5        $ rbFemale��5D   "   $��Kadın��64   6D   5E      �� 5H      $��E-posta:��6   #��[  6,   �� 5I      -   5        $ txtEmail��5I      #��[ 6,   6O   �� 5J      -	   5        $
 txtEmailError��5J      
���  ��$y�	Collapsed=�6D   $*��$Geçerli bir e-posta adresi giriniz.��6[   $��12(�6�   $��0,-4,0,8q�6�   �� 5M      $��Şifre:��6   #��[  6*   G� 5N      -
   5         G�Name$ txtPassword��5N      #��[ 63   6V   �� 5Q      $��Şifre Tekrar:��6   #��[  61   G� 5R      -   5        $ txtConfirmPassword��5R      #��[ 6:   6]   �� 5S      -   5        $
 txtPasswordError��5S      
���  ��$y�	Collapsed=�6G   $��Şifreler eşleşmiyor.��6^   $��12(�6z   $��0,-4,0,8q�6�   �� 5V      $��Diyabet Tipi:��6   #��[  61   �� 5W      -   5         ��Name$ cmbDiabetesType��5W      #��[ 64   .@�5X      �� $��Tip 1 Diyabet��6#   �� 5Y      $��Tip 2 Diyabet��6#   �� 5Z      $��Gestasyonel Diyabet��6#   �� 5[      $��Diğer��6#   5\      �� 5_      $��Tanı Tarihi:��6   #��[  60     5`      -   5        $ dpDiagnosisDate��5`      #��[ 66   6Y   �� 5c      $��Doktor Notları:��6   #��[  63   �� 5d      -   5        $ txtNotes��5d      $��80��6,    �� TextWrapping$
 Wrap=�68    �� AcceptsReturn . " �� VerticalScrollBarVisibility$
 Auto=�6a   5e      5f      � 5i   
   $��2��6   $��0,20,0,0q�6    � ColumnDefinitions 5j      �� 5k      $��* �6#   �� 5l      $��* �6#   .+�5o      �� -   5        $��0��5o      $��İptal��6%   #��[ 65   ���#EEEEEE6n   �� 5p      -   5         ��Name$ btnSave��5p   %   $��1��6   $��Kaydet��66   #��[ 6G   ���#2196F3
�������{�.  6�   5q      5r      5s      