   M S B A M L   `   `   `  ����O  Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem  5   	   E Awpf prolab, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:wpf_prolab.UIwpf_prolab.UI 5   	   U QLiveCharts.Wpf, Version=0.9.7.0, Culture=neutral, PublicKeyToken=0bc1f845d1ebb8dfG4clr-namespace:LiveCharts.Wpf;assembly=LiveCharts.WpfLiveCharts.Wpf 5   	   #   wpf_prolab.UI.DoctorDashboard   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   9System&clr-namespace:System;assembly=mscorlib     5   	   -localclr-namespace:wpf_prolab.UI     5   	   Dlvc4clr-namespace:LiveCharts.Wpf;assembly=LiveCharts.Wpf     5   	     � Title$+  %Diyabet Takip Sistemi - Doktor Paneli��5
   	   $	��700��67   $
��1100��6D    � WindowStartupLocation$ CenterScreen=�5   	    � 	Resources 5          BooleanToVisibilityConverter �� 
TargetType  
HeaderText �� Property �� Value  SubHeaderText  ActionButton��%�  &	        &	      &	 �     &	 �    �� 5   
   6'   �� 5   
   " ��6   .�5      ��  � ��$ 22(�6)   �� 5       � ��$
 Bold"�6+   �� 5       � �� �#2196F36+   �� 5       � ��$ 0,0,0,10q�6'   5      �� 5   
   " ��6   .�5      ��  � ��$ 16(�6)   �� 5       � ��$ SemiBold"�6+   �� 5       � �� �#5555556+   �� 5       � ��$ 0,10,0,5q�6'   5      �� 5   
   " ��6   .�5      ��  � �� �#2196F36+   �� 5        � ��
 �����6+   �� 5!       � ��$
 15,8q�6(   �� 5"       � ��$ 0q�60   �� 5#       � ��$ 5q�6'   �� 5$       �@�Cursor$
 Hand��6'   5%      5&      .��5(      �  � RowDefinitions 5)   
   �� 5*      $
��Auto �6   �� 5+      $��* �6   .+�5/   
   �� $��0��6   ���#2196F3 �� Padding$ 20,15q�63   ._�50      �  � ColumnDefinitions 51      �� 52      $��* �6'   �� 53      $
��Auto �6'   .+�56      �� $��0��6   	 �� Orientation$	 
Horizontal=�6-   .+�57      �� $��Diyabet Takip Sistemi��6    $��22(�6=   $
��Bold"�6K   
�������6]   �� 58      $��Doktor Paneli��6    $��16(�65   
�������$��Center=�6V   $��10,0,0,0q�6q   59      �� 5;      $��1��6   $	 
Horizontal=�6-   .+�5<      �� -   5        
 ��Name$
 txtDoctorName��5<       $��Dr. Ad Soyad��67   
�������$��Center=�6^   $��0,0,15,0q�6y   �� 5=      -   5         ��Name$ 	btnLogout��5=      $��	Çıkış��60   
������ $��1q�6Y   
�������
�������$
��10,5q�5>   0   6?   5?      5@      5A      � 5D   
   $��1��6    5E      �� 5F      $	��250 �6#   �� 5G      $��* �6#   .+�5K      �� $��0��6   ���#F5F5F5$��0,0,1,0q�6:   ���#DDDDDD._�5L      �  5M      �� 5N      $
��Auto �6(   �� 5O      $
��Auto �6(   �� 5P      $��* �6(   �� 5Q      $
��Auto �6(   .+�5U      � $��0��6   $��10q�6(    5V      �� 5W      $��* �6/   �� 5X      $
��Auto �6/   .+�5Z      �� -   5         ��Name$ 	txtSearch��5Z   "   $��0��65   $��5q�6E   .��6u   ��5[      �� 5\   "   " ��6(   .�5]   &   ��  � �� 5^   *   �� 5_   .    �� 
TargetType" ��6>   .R�5`   2   �� #��z��69   #��z��6c   #��z��6�   ._�5a   6   � .+�5b   :   �� $��Hasta ara...��6D   
�������$��5,3,0,0q�6j   y�6{   ��  �� RelativeSource �� *TemplatedParent+ �� Path$ Text.IsEmpty� �� 	Converter# [  �� 5c   :    ��Name$ PART_ContentHost��6G   $��5,3,0,0q�6a   5d   7   5e   3   5f   /   5h   '   5i   #   .��5j   -   5k      �� 5l      $��1��6!   $
��🔍��61   $��30��6>   $��0q�6I   
������ 6]   5m      �� 5p      $��1��6!   $��
10,0,10,10q�6.   .+�5q      �� $��Kan Şekeri Seviyesi:��6$   $��SemiBold"�6@   �� 5r      -   5         ��Name$ cmbGlucoseFilter��5r   #   $��0,5,0,0q�6=   .@�5s      �� $��Tümü��6+    �� 
IsSelected . 6:   �� 5t      $��Düşük (< 70 mg/dL)��6+   �� 5u      $��Normal (70-110 mg/dL)��6+   �� 5v      $��Yüksek (111-180 mg/dL)��6+   �� 5w      $ ��Çok Yüksek (> 180 mg/dL)��6+   5x      5y      �� 5|      -   5         ��Name$ lstPatients��5|      $��2��64   $
��10,0q�6A   $��0q�6O    �� DisplayMemberPath$ FullName��5}   O   5~      �� 5�      -   5        $��3��5�      $��+ Yeni Hasta Ekle��6*   #��[ 6F   $��10q�6l   5�      5�      5�      �� 5�      -   5         ��Name$ tabPatientDetails��5�   )   $��1��6   $��10q�6D   .@�5�      �� $��Genel Bilgiler��6   .��5�      �� $
��Auto=�6#   .��5�      � $��20q�6    5�      �� 5�   "   $
��Auto �60   �� 5�   "   $
��Auto �60   �� 5�   "   $��* �60   .+�5�      � $��0��6#    5�   "   �� 5�   &   $
��Auto �67   �� 5�   &   $��* �67   .+�5�   "   �� $��0��6)   $��80��69   $��80��6D   
������� �� CornerRadius$ 40��6g   $��0,0,20,0q�6y   ._�5�   &   �� -   5        $
 txtPatientInitials��5�   0   $��AA��6L   $��30(�6V   $
��Bold"�6d   $��Center=�5�   .   $��Center=�6K   5�   #   �� 5�   "   $��1��6-   .+�5�   &   �� -	   5        $
 txtPatientName��5�   0   $��Hasta Adı Soyadı��6H   #��[ 6`   $��0q�6�   �� 5�   &   -
   5        $
 txtPatientDetails��5�   0   $'��!TC: 12345678901 | 40 yaş | Erkek��6K   $��14(�5�   .   ���#6666666<   5�   #   5�      � 5�      $��1��6#   $��0,20,0,0q�60    5�   "   �� 5�   &   $��* �67   �� 5�   &   $��* �67   �� 5�   &   $��* �67   .+�5�   "   �� $��0��6)   ���#E3F2FD$ 15q�6N   $��0,0,10,0q�6[   $ 5��6m   ._�5�   &   �� .+�5�   *   �� $��Son Kan Şekeri��64   $��SemiBold"�6J   �� 5�   *   -   5        $
 txtLastGlucose��5�   4   $��	120 mg/dL��6L   $��22(�6]   $
��Bold"�6k   ���#2196F36}   �� 5�   *   -   5        $
 txtLastGlucoseTime��5�   4   $��Bugün 10:25��6P   $��12(�6c   ���#6666666q   5�   '   5�   #   �� 5�   "   $��1��6)   ���#E8F5E9$ 15q�6N   $	��5,0q�6[   $ 5��6h   ._�5�   &   �� .+�5�   *   �� $��Diyet Uyumu��64   $��SemiBold"�6G   �� 5�   *   -   5        $
 txtDietAdherence��5�   4   $	��%85��6N   $��22(�6Y   $
��Bold"�6g   ���#4CAF506y   �� 5�   *   $��Son 30 gün��64   $��12(�6F   ���#6666666T   5�   '   5�   #   �� 5�   "   $��2��6)   ���#FFF3E0$ 15q�6N   $��10,0,0,0q�6[   $ 5��6m   ._�5�   &   �� .+�5�   *   �� $��Egzersiz Uyumu��64   $��SemiBold"�6J   �� 5�   *   -   5        $
 txtExerciseAdherence��5�   4   $	��%70��6R   $��22(�6]   $
��Bold"�6k   ���#FF98006}   �� 5�   *   $��Son 30 gün��64   $��12(�6F   ���#6666666T   5�   '   5�   #   5�      � 5�      $��2��6#   $��0,20,0,0q�60    5�   "   �� 5�   &   $
��Auto �64   �� 5�   &   $��* �64   �� 5�   &   $
��Auto �64   �� 5�   &   $��* �64   .+�5�   "   �� $��0��6,   $��Son Ölçümler��69   #��[ 6M   &   System.Windows.Controls.DataGrid  5�   "   -   5          Name$ gridRecentMeasurements��5�   8   $��1��6+      AutoGenerateColumns .     
IsReadOnly . $��	0,10,0,20q�5�   +      HeadersVisibility$ Column=�6>      GridLinesVisibility$ 
Horizontal=�6Y      Columns 5�   &   0  *System.Windows.Controls.DataGridTextColumn  5�   *   ,  &System.Windows.Controls.DataGridColumn   Header$ Tarih��6=      Width5  /System.Windows.Controls.DataGridLengthConverter$	 100 6�   1  +System.Windows.Controls.DataGridBoundColumn    Binding  6L   �� *MeasurementTime+! �� StringFormat$! {0:dd.MM.yyyy}��6�     5�   *   $
 Saat��6=   $ 80 6�     6K   �� *MeasurementTime+$! 	{0:HH:mm}��6�     5�   *   $
 Tür��6=   $	 100 6n     6J   �� *MeasurementType+6n     5�   *   $ Değer (mg/dL)��6=   $	 120 6y     6T   �� *MeasurementValue+6y   4  .System.Windows.Controls.DataGridTemplateColumn  5�   *   $ Durum��6A   $	 120 6P   "   CellTemplate" 5�   .   �� 5�   2   .R�5�   6   �� $ 3��6`   $	 5,2q�6q   ��6=   �� *StatusColor+._�5�   :   �� 
�������$��Center=�6s   ��6D   �� *
StatusText+6s   5�   7   5�   3   5�   +   5�   #   �� 5�   "   $��2��6,   $��Son Uyarılar��69   #��[ 6M     5�   "   -   5        $ gridRecentAlerts��5�   8   $��3��6+    .   . $��0,10,0,0q�5�   +   $ Column=�6=   $ 
Horizontal=�6X    5�   &     5�   *   $ Tarih��6=   $	 150 6�     6L   �� *	CreatedAt+$! {0:dd.MM.yyyy HH:mm}��6�     5�   *   $ Uyarı Tipi��6=   $	 150 6o     6Q   �� *	AlertType+6o     5�   *   $ Mesaj��6=   $ * 6m     6L   �� *AlertMessage+6m     5�   *   $ Durum��6A   $	 100 6P   " 5�   .   �� 5�   2   .R�5�   6   �� ��6@   �� *
IsReadText+��6\   �� *IsReadColor+5�   3   5�   +   5�   #   5�      5�      5�      5�      �� 5�      $��Kan Şekeri Takibi��6   .��5�      � $��20q�6    5�      �� 5�      $
��Auto �6,   �� 5      $
��Auto �6,   �� 5     $
��Auto �6,   �� 5     $��* �6,   .+�5     �� $��0��6$   $$��Kan Şekeri Değişim Grafiği��61   #��[ 6S   �� 5     $��1��6%   $	 
Horizontal=�62   $��	0,10,0,20q�6K   .+�5	     �� $��Tarih Aralığı:��6(   $��Center=�6>   (  "System.Windows.Controls.DatePicker  5
     -   5        #  Name$# dpGlucoseStart��5
  )   $
��10,0q�6A   6O   �� 5     $��-��6(   $��Center=�61     5     -   5        $# dpGlucoseEnd��5  )   $
��10,0q�6?   6M   �� 5     -   5        $��Uygula��5  %   #��[ 66   6\   5     �� 5     $��2��6!   ���#F5F5F5$ 15q�6C   $��0,0,0,20q�6P   $ 5��6b   ._�5     �  5  "   �� 5  &   $
��Auto �64   �� 5  &   $
��Auto �64   .+�5  "   �� $��0��6,   $#��Kan Şekeri Ölçümü Kaydet��69   $��SemiBold"�6Y   $��0,0,0,10q�6o   � 5  "   $��1��6'    5  &   �� 5  *   $
��Auto �68   �� 5  *   $
��Auto �68    5  &   �� 5   *   $
��Auto �6;   �� 5!  *   $
��Auto �6;   �� 5"  *   $
��Auto �6;   �� 5#  *   $
��Auto �6;   .+�5'  &   �� $��0��61   $��0��6>   $��	0,0,10,10q�6N   .+�5(  *   �� $��Değer (mg/dL)��64   $��0,0,0,5q�6I   �� 5)  *   -   5        $ txtGlucoseValue��5)  2   $	��100��6K   $ �� 	MaxLength$$ 6��6W   6e   5*  '   �� 5,  &   $��0��61   $��1��6>   $��	0,0,10,10q�6N   .+�5-  *   �� $��Zaman��64   $��0,0,0,5q�6A     5.  *   -   5        $# dpGlucoseDate��5.  5   $	��120��6L   5/  '   �� 51  &   $��0��61   $��2��6>   $��	0,0,10,10q�6N   .+�52  *   �� $
��Saat��64   $��0,0,0,5q�6@   �� 53  *   -   5        $ txtGlucoseTime��53  2   $��80��6J   $��12:00��6U   54  '   �� 56  &   $��0��61   $��3��6>   $��	0,0,10,10q�6N   .+�57  *   �� $��Ölçüm Türü��64   $��0,0,0,5q�6F   �� 58  *   -   5        $ cmbMeasurementType��58  3   $	��120��6O   .@�59  .   �� $��Sabah��6;   
% � Tag$% Morning��6K   �� 5:  .   $��Öğle��6;   $
% Noon��6J   �� 5;  .   $��İkindi��6;   $% 	Afternoon��6L   �� 5<  .   $��Akşam��6;   $% Evening��6K   �� 5=  .   $
��Gece��6;   $% Night��6J   5>  +   5?  '   �� 5B  &   $��1��61   $��0��6>   $��0,0,10,0q�6N   .+�5C  *   �� $��İnsülin Dozu (ml)��64   $��0,0,0,5q�6M   �� 5D  *   -   5        $ txtInsulinDose��5D  2   $	��100��6J   $$ 5��6V   6d   5E  '   �� 5G  &   $��1��61   $��1��6>   $��2��6N   $��0,0,10,0q�6b   .+�5H  *   �� $��Notlar��64   $��0,0,0,5q�6B   �� 5I  *   -   5        $ txtGlucoseNotes��5I  2   $	��210��6K   5J  '   �� 5L  &   $��1��61   $��3��6>   $��0,0,10,0q�6N   $��Right=�6`   .+�5M  *   �� $�� ��64   $��0,0,0,5q�6=   $y�Hidden=�6N   �� 5N  *   -   5        $ btnSaveGlucose��5N  1   $��Kaydet��6I   $	��100��6Z   $��30��6f   #��[ 5O  1   5P  1   5Q  '   5R  #   5S     5T     � 5W     $��3��6    5X     �� 5Y  "   $��* �60   �� 5Z  "   $
��Auto �60   .+�5^     #  LiveCharts.Wpf.CartesianChart  -   5        &  Name$& chartBloodGlucose��5^  1   $��0��6L   &	   LiveCharts.Wpf.Charts.Base.Chart' 	  LegendLocation$' Right=�5_  /   ( 	  DisableAnimations( . ) 	  Series) 5^  Y   �� *GlucoseSeries+* 	  AxisX* 5a  &   
  LiveCharts.Wpf.Axis
  + 
  Title$+ 
Tarih/Saat��6/   , 
  LabelFormatter, 6B   �� *DateTimeFormatter+- 	  AxisY- 5d  &   
  $+ Kan Şekeri (mg/dL)��6/   . 
  MinValue$. 0Q�6J   / 
  Sections/ 5g  .      LiveCharts.Wpf.AxisSection  0   Value$0 0Q�6>   1   SectionWidth$1 70Q�6H   2   Fill2 �#FFE8E86Z     5h  .   $0 70Q�6>   $1 40Q�6I   2 �#E8F5E86[     5i  .   $	0 110Q�6>   $1 70Q�6J   2 �#FFF7E66\     5j  .   $	0 180Q�6>   $	1 120Q�6J   2 �#FFDED66]   5l  '   5n     �� 5q     $��1��6)   $	 
Horizontal=�66   $��Center=�6O   $��0,10,0,0q�6l   .+�5r  "   �� ���#FFE8E8$ 3��6>   $	 5,2q�6O   $	��5,0q�6]   ._�5s  &   �� $��Düşük (< 70 mg/dL)��60   5t  #   �� 5u  "   ���#E8F5E8$ 3��6>   $	 5,2q�6O   $	��5,0q�6]   ._�5v  &   �� $��Normal (70-110 mg/dL)��60   5w  #   �� 5x  "   ���#FFF7E6$ 3��6>   $	 5,2q�6O   $	��5,0q�6]   ._�5y  &   �� $��Yüksek (111-180 mg/dL)��60   5z  #   �� 5{  "   ���#FFDED6$ 3��6>   $	 5,2q�6O   $	��5,0q�6]   ._�5|  &   �� $ ��Çok Yüksek (> 180 mg/dL)��60   5}  #   5~     5     5�     5�     �� 5�     $��Diyet ve Egzersiz��6   .��5�     � $��20q�6    5�     �� 5�     $
��Auto �6,   �� 5�     $��* �6,   �� 5�     $
��Auto �6,   �� 5�     $��* �6,   .+�5�     � $��0��6    5�     �� 5�  "   $��* �63   �� 5�  "   $
��Auto �63   .+�5�     �� $��0��6(   $��Diyet Planı��68   #��[ 6K   �� 5�     -   5        $��1��5�  %   $��Yeni Diyet Planı��65   #��[ 6P   5�  #   5�     � 5�     -   5        3 �Name$3 gridDietPlans��5�  ,   $��1��6   $��	0,10,0,20q�6C   $y�Visible=�6V   .+�5�       -   5        $ dgDietPlans��5�  '    .   . $ Column=�5�  %   $ 
Horizontal=�6@    5�  "     5�  &   $ Başlangıç��69   $	 100 6�     6L   �� *	StartDate+$! {0:dd.MM.yyyy}��6�     5�  &   $ Bitiş��69   $	 100 6k     6H   �� *EndDateDisplay+6k     5�  &   $ Diyet Türü��69   $	 150 6q     6M   �� *DietTypeDisplay+6q     5�  &   $ Doktor Notu��69   $ * 6n     6N   �� *DoctorNotes+6n     5�  &   $ 
İşlemler��6=   $	 100 6O   " 5�  *   �� 5�  .   .R�5�  2   �� -   5        $��Düzenle��5�  9   % 6e   �� *Id+5�  /   5�  '   5�     5�     � 5�     -    5        $3 panelAddDiet��5�  ,   $��1��6   $y�	Collapsed=�6B   $��	0,10,0,20q�6Y   .+�5�     �� ���#F5F5F5$ 20q�6:   $ 5��6G   ._�5�  "   �  5�  &   �� 5�  *   $
��Auto �68   �� 5�  *   $
��Auto �68   �� 5�  *   $
��Auto �68   �� 5�  *   $
��Auto �68   �� 5�  *   $
��Auto �68    5�  &   �� 5�  *   $	��120 �6;   �� 5�  *   $��* �6;   .+�5�  &   �� $��0��60   $��0��6=   $��2��6M   $��Yeni Diyet Planı��5�  .   $
��Bold"�6F   $��16(�6X   $��0,0,0,15q�6f   �� 5�  &   $��1��60   $��0��6=   $��Diyet Türü:��6M   $��Center=�6`   �� 5�  &   -!   5        $ cmbDietType��5�  L   $��1��6/   $��1��6<   $��0,5,0,10q�6a   $	��250��6s   $
��Left=�6   .@�5�  *   �� $��Az Şekerli Diyet��67   $% 0��6R   4 � ToolTip$�4 �Şekerli gıdalar sınırlanır, kompleks karbonhidratlara öncelik verilir. Lifli gıdalar ve düşük glisemik indeksli besinler tercih edilir.��6Z   �� 5�  *   $��Şekersiz Diyet��67   $% 1��6P   $~4 xRafine şeker ve şeker katkılı tüm ürünler tamamen dışlanır. Hiperglisemi riski taşıyan bireylerde önerilir.��6X   �� 5�  *   $��Dengeli Beslenme��67   $% 2��6R   $�4 �Diyabetli bireylerin yaşam tarzına uygun, dengeli ve sürdürülebilir bir diyet yaklaşımıdır. Tüm besin gruplarından yeterli miktarda alınır; porsiyon kontrolü, mevsimsel taze ürünler ve su tüketimi temel unsurlardır.��6Z   5�  '   �� 5�  &   $��2��60   $��0��6=   $��Başlangıç Tarihi:��6M   $��Center=�6f     5�  &   -"   5        $# dpDietStartDate��5�  N   $��2��61   $��1��6>   $��0,5,0,10q�6g   $	��150��6y   $
��Left=�6�   �� 5�  &   $��3��60   $��0��6=   $��Bitiş Tarihi:��6M   $��Center=�6b   �� 5�  &   $��3��61   $��1��6>   $	 
Horizontal=�6N   $��0,5,0,10q�6g   .+�5�  *     -#   5        $# dpDietEndDate��5�  5   $	��150��6L   �� 5�  *   $8��2(Boş bırakılırsa süresiz olarak kabul edilir)��64   $��10,0,0,0q�6i   $��Center=�6{   ���#5556�   5�  '   �� 5�  &   $��4��60   $��0��6=   $��Doktor Notu:��6M   $	��Top=�6a   $��0,5,0,0q�6y   �� 5�  &   -$   5        $ txtDietNotes��5�  K   $��4��6.   $��1��6;   $��0,5,0,0q�6a   $��80��5�  ,   5 �� TextWrapping$
5 Wrap=�68   6 �� AcceptsReturn6 . 6L   �� 5�  &   $��5��61   $��1��6>   $	 
Horizontal=�6N   $��Right=�6g   $��0,15,0,0q�6�   .+�5�  *   �� -%   5        $��İptal��5�  1   $
��15,8q�6A   $��0,0,10,0q�6P   6b   �� 5�  *   -&   5        $��Kaydet��5�  1   #��[ 6B   6h   5�  '   5�  #   5�     5�     � 5�     $��2��6    5�     �� 5�  "   $��* �63   �� 5�  "   $
��Auto �63   .+�5�     �� $��0��6(   $��Egzersiz Planı��68   #��[ 6N   �� 5�     -'   5        $��1��5�  %   $��Yeni Egzersiz Planı��65   #��[ 6S   5�  #   5�     � 5�     -(   5        $3 gridExercisePlans��5�  ,   $��3��6   $��0,10,0,0q�6G   $y�Visible=�6Y   .+�5�       -)   5        $ dgExercisePlans��5�  '    .   . $ Column=�5�  %   $ 
Horizontal=�6@    5�  "     5�  &   $ Başlangıç��69   $	 100 6�     6L   �� *	StartDate+$! {0:dd.MM.yyyy}��6�     5�  &   $ Bitiş��69   $	 100 6k     6H   �� *EndDateDisplay+6k     5�  &   $ Egzersiz Türü��69   $	 150 6x     6P   �� *ExerciseTypeDisplay+6x     5�  &   $ Doktor Notu��69   $ * 6n     6N   �� *DoctorNotes+6n     5�  &   $ 
İşlemler��6=   $	 100 6O   " 5�  *   �� 5�  .   .R�5�  2   �� -*   5        $��Düzenle��5�  9   % 6i   �� *Id+5�  /   5�  '   5�     5�     � 5�     -+   5        $3 panelAddExercise��5�  ,   $��3��6   $y�	Collapsed=�6F   $��0,10,0,0q�6]   .+�5�     �� ���#F5F5F5$ 20q�6:   $ 5��6G   ._�5�  "   �  5�  &   �� 5�  *   $
��Auto �68   �� 5�  *   $
��Auto �68   �� 5�  *   $
��Auto �68   �� 5�  *   $
��Auto �68   �� 5   *   $
��Auto �68    5  &   �� 5  *   $	��120 �6;   �� 5  *   $��* �6;   .+�5  &   �� $��0��60   $��0��6=   $��2��6M   $��Yeni Egzersiz Planı��5  .   $
��Bold"�6I   $��16(�6[   $��0,0,0,15q�6i   �� 5
  &   $��1��60   $��0��6=   $��Egzersiz Türü:��6M   $��Center=�6c   �� 5  &   -,   5        $ cmbExerciseType��5  L   $��1��6/   $��1��6<   $��0,5,0,10q�6e   $	��250��6w   $
��Left=�6�   .@�5  *   �� $��Yürüyüş��67   $% 0��6I   $=4 7Hafif tempolu, günlük yapılabilecek bir egzersizdir.��6Q   �� 5  *   $��Bisiklet��67   $% 1��6J   $b4 \Alt vücut kaslarını çalıştırır ve dış mekanda veya sabit bisikletle uygulanabilir.��6R   �� 5  *   $��Klinik Egzersiz��67   $% 2��6Q   $�4 �Doktor tarafından verilen belirli hareketleri içeren planlı egzersizlerdir. Stresi azaltılması ve hareket kabiliyetinin artırılması amaçlanır.��6Y   5  '   �� 5  &   $��2��60   $��0��6=   $��Başlangıç Tarihi:��6M   $��Center=�6f     5  &   --   5        $# dpExerciseStartDate��5  N   $��2��61   $��1��6>   $��0,5,0,10q�6k   $	��150��6}   $
��Left=�6�   �� 5  &   $��3��60   $��0��6=   $��Bitiş Tarihi:��6M   $��Center=�6b   �� 5  &   $��3��61   $��1��6>   $	 
Horizontal=�6N   $��0,5,0,10q�6g   .+�5  *     -.   5        $# dpExerciseEndDate��5  5   $	��150��6P   �� 5  *   $8��2(Boş bırakılırsa süresiz olarak kabul edilir)��64   $��10,0,0,0q�6i   $��Center=�6{   ���#5556�   5  '   �� 5  &   $��4��60   $��0��6=   $��Doktor Notu:��6M   $	��Top=�6a   $��0,5,0,0q�6y   �� 5  &   -/   5        $ txtExerciseNotes��5  K   $��4��6.   $��1��6;   $��0,5,0,0q�6e   $��80��5  ,   $
5 Wrap=�68   6 . 6L   �� 5  &   $��5��61   $��1��6>   $	 
Horizontal=�6N   $��Right=�6g   $��0,15,0,0q�6�   .+�5  *   �� -0   5        $��İptal��5  1   $
��15,8q�6A   $��0,0,10,0q�6P   6b   �� 5   *   -1   5        $��Kaydet��5   1   #��[ 6B   6h   5!  '   5"  #   5#     5$     5%     5&     �� 5(     $��
Semptomlar��6   .��5)     � $��20q�6    5*     �� 5+     $
��Auto �6,   �� 5,     $
��Auto �6,   �� 5-     $��* �6,   .+�50     �� $��0��6$   $��Semptom Geçmişi��61   #��[ 6H   �� 53     $��1��6%   $	 
Horizontal=�62   $��	0,10,0,20q�6K   .+�54     �� $��Tarih Aralığı:��6(   $��Center=�6>     55     -2   5        $# dpSymptomsStart��55  )   $
��10,0q�6B   6P   �� 56     $��-��6(   $��Center=�61     57     -3   5        $# dpSymptomsEnd��57  )   $
��10,0q�6@   6N   �� 58     -4   5        $��Uygula��58  %   #��[ 66   6\   �� 59     -5   5        $��Yeni Semptom Ekle��59  %   #��[ 6A   $��20,0,0,0q�6�   5:     � 5=     -6   5        $3 panelAddDoctorSymptom��5=  ,   $��2��6   $y�	Collapsed=�6K   $��0,0,0,20q�6b    5>     �� 5?  "   $	��150 �63   �� 5@  "   $	��150 �63   �� 5A  "   $��* �63   �� 5B  "   $	��100 �63    5D     �� 5E  "   $
��Auto �60   �� 5F  "   $
��Auto �60   �� 5G  "   $
��Auto �60   .+�5J     �� $��0��6(   $��0��65   $��Semptom Tarihi:��6E   $��Center=�6\     5K     -7   5        $# dpDoctorSymptomDate��5K  Z   $��0��6)   $��1��66   $��2��6F   $��0,5,0,10q�5L  )   $
��Left=�6;   $	��150��6V   �� 5N     $��1��6(   $��0��65   $��Semptom Tipi:��6E   $��Center=�6Z   �� 5O     $��1��6(   $��1��65   $��Şiddet:��6E   $��Center=�6T   �� 5P     $��1��6(   $��2��65   $��Notlar:��6E   $��Center=�6T   �� 5R     -8   5        $ cmbDoctorSymptomType��5R  D   $��2��6'   $��0��64   $��0,5,10,5q�6b   .@�5T  "   �� $$��Poliüri (Sık idrara çıkma)��6/   �� 5U  "   $'��!Polifaji (Aşırı açlık hissi)��6/   �� 5V  "   $'��!Polidipsi (Aşırı susama hissi)��6/   �� 5W  "   $��	Nöropati��6/   �� 5X  "   $��Kilo kaybı��6/   �� 5Y  "   $��	Yorgunluk��6/   �� 5Z  "   $#��Yaraların yavaş iyileşmesi��6/   �� 5[  "   $��Bulanık görme��6/   5\     �� 5^     -9   5        $ cmbDoctorSymptomIntensity��5^  D   $��2��6'   $��1��64   $��0,5,10,5q�6g   .@�5`  "   �� $��	Hafif (1)��6/   $% 1��6C   �� 5a  "   $��Orta (2)��6/   $% 2��6B   �� 5b  "   $��Şiddetli (3)��6/   $% 3��6F   �� 5c  "   $��Çok Şiddetli (4)��6/   $% 4��6J   �� 5d  "   $��Dayanılmaz (5)��6/   $% 5��6H   5e     �� 5g     -:   5        $ txtDoctorSymptomNotes��5g  C   $��2��6&   $��2��63   $��0,5,10,5q�6b   �� 5i     $��2��6)   $��3��66   $	 
Horizontal=�6F   .+�5j  "   �� -;   5        $��Kaydet��5j  )   #��[ 6:   $��0,5,5,5q�6`   5k  )   �� 5l  "   -<   5        $��İptal��5l  )   #��[ 69   $��5,5,0,5q�6_   5m  )   5n     5o       5q     -=   5        $ gridSymptoms��5q  0   $��2��6#    .   . $��0,10,0,0q�5r  !   $ Column=�63   $ 
Horizontal=�6N    5s       5t  "   $ Tarih��65   $	 100 6�     6D   �� *SymptomDate+$! {0:dd.MM.yyyy}��6�     5u  "   $ Semptom��65   $	 150 6i     6F   �� *SymptomDisplay+6i     5v  "   $ Şiddet��65   $ 80 6c     6E   �� *	Intensity+6c     5w  "   $ Notlar��65   $ * 6_     6E   �� *Notes+6_   5y     5z     5{     �� 5}     $��	Uyarılar��6   .��5~     � $��20q�6    5     �� 5�     $
��Auto �6,   �� 5�     $
��Auto �6,   �� 5�     $��* �6,   .+�5�     �� $��0��6$   $��Tüm Uyarılar��61   #��[ 6E   �� 5�     $��1��6%   $	 
Horizontal=�62   $��	0,10,0,20q�6K   .+�5�     �� $��Göster:��6(   $��Center=�67   �� 5�     ->   5        $ cmbAlertFilter��5�  '   $	��150��6?   $
��10,0q�6K   .@�5�  "   �� $��Tüm Uyarılar��6/    . 6F   �� 5�  "   $��Okunmamış Uyarılar��6/   �� 5�  "   $��Acil Uyarılar��6/   5�     �� 5�     -?   5        $!��Tümünü Okundu İşaretle��5�  %   #��[ 6F   5�  #   5�       5�     -@   5        $ gridAllAlerts��5�  0   $��2��6#    .   . $��0,10,0,0q�5�  !   $ Column=�63   $ 
Horizontal=�6N    5�       5�  "   $ Tarih��65   $	 150 6�     6D   �� *	CreatedAt+$! {0:dd.MM.yyyy HH:mm}��6�     5�  "   $ Uyarı Tipi��65   $	 150 6g     6I   �� *	AlertType+6g     5�  "   $ Mesaj��65   $ * 6e     6D   �� *AlertMessage+6e     5�  "   $ Durum��69   $	 100 6H   " 5�  &   �� 5�  *   .R�5�  .   �� ��68   �� *
IsReadText+��6T   �� *IsReadColor+5�  +   5�  #     5�  "   $ İşlem��69   $	 100 6H   " 5�  &   �� 5�  *   .R�5�  .   �� -A   5        $��Okundu��5�  5   % 6e   �� *Id+y�5�  3   �� *IsReadButtonVisibility+5�  +   5�  #   5�     5�     5�     �� 5�     $��🎯 Hasta Önerileri��6   .��5�     � $��20q�6    5�     �� 5�     $
��Auto �6,   �� 5�     $
��Auto �6,   �� 5�     $
��Auto �6,   �� 5�     $��* �6,   .+�5�     �� $��0��6$   $"��Akıllı Sağlık Önerileri��61   #��[ 6P   � 5�     $��1��6   $��	0,10,0,20q�6,    5�     �� 5�  "   $
��Auto �63   �� 5�  "   $	��200 �63   �� 5�  "   $
��Auto �63   �� 5�  "   $��* �63   .+�5�     �� $��0��6(   $��Hasta:��68   $��Center=�6F   $��SemiBold"�6a   �� 5�     -B   5        $ cmbRecommendationPatient��5�  7   $��1��6'   $
��10,0q�6Y   $ FullName��5�  &   7 �� SelectedValuePath$7 Id��6C   5�  &   �� 5�     -C   5        $% btnRefreshDoctorRecommendations��5�  5   $��2��6%   $��🔄 Önerileri Yenile��6^   #��[ 5�  %   $
��10,0q�6y   �� 5�     -D   5        $#
 txtRecommendationsLastUpdated��5�  8   $��3��6(   $��Son güncelleme: --��6_   $��Center=�5�  (   $��Italic$�6C   5�     �� 5�     -E   5        8 ��Name$$8 panelRecommendationPatientInfo��5�  !   $��2��5�  !   ���#F8F9FA$ 15q�6C   $��0,0,0,20q�6P   $ 5��6b   $y�	Collapsed=�5�  I   ._�5�     �  5�  "   �� 5�  &   $��* �67   �� 5�  &   $��* �67   �� 5�  &   $��* �67   �� 5�  &   $��* �67   .+�5�  "   �� $��0��6-   .+�5�  &   �� $��Son Ölçüm��60   $��SemiBold"�6A   �� 5�  &   -F   5        $"
 txtRecommendationLastGlucose��5�  0   $��--��6V   $��18(�6`   ���#2196F36n   5�  #   �� 5�  "   $��1��6-   .+�5�  &   �� $��Günlük Ortalama��60   $��SemiBold"�6G   �� 5�  &   -G   5        $
 txtRecommendationDailyAvg��5�  0   $��--��6S   $��18(�6]   ���#4CAF506k   5�  #   �� 5�  "   $��2��6-   .+�5�  &   �� $��Toplam Ölçüm��60   $��SemiBold"�6D   �� 5�  &   -H   5        $(
 "txtRecommendationTotalMeasurements��5�  0   $��--��6\   $��18(�6f   ���#FF98006t   5�  #   �� 5�  "   $��3��6-   .+�5�  &   �� $��Risk Seviyesi��60   $��SemiBold"�6E   �� 5�  &   -I   5        $ 
 txtRecommendationRiskLevel��5�  0   $��--��6T   $��18(�6^   $
��Bold"�6l   5�  #   5�     5�     �� 5�     -J   5        $ scrollRecommendations��5�  '   $��3��5�  '   $
��Auto=�64   $y�	Collapsed=�5�  F   .��5�     �� -K   5        9 ��Name$9 lstDoctorRecommendations��5�  +   ��5�  "   �� 5�  &   .R�5�  *   �� $��0,0,0,15q�61   $ 8��6C   $ 15q�6T   ��5�  1   �� *PriorityBrush+._�5�  .   �  5�  2   �� 5�  6   $
��Auto �6D   �� 5�  6   $
��Auto �6D   �� 5�  6   $
��Auto �6D   �� 5�  6   $
��Auto �6D   �� 5�  6   $
��Auto �6D   .+�5�  2   � $��0��67    5   6   �� 5  :   $��* �6K   �� 5  :   $
��Auto �6K   �� 5  :   $
��Auto �6K   .+�5  6   �� $��0��6@   $
��Bold"�5  @   $��16(�6R   
���������5  P   �� *Title+5  `   �� 5  6   $��1��6@   $��12(�5	  @   
�������$��Italic$�6a   $
��10,0q�6t   ��5  P   �� *
Category+5	  t   �� 5
  6   $��2��6=   ���#000000$	 5,2q�6b   $ 3��6p   ._�5  :   �� $��12(�5  D   $
��Bold"�6R   
���������5  D   �� *PriorityText+5  d   5  7   5  3   �� 5  2   $��1��6<   $��0,8,0,10q�5  <   $
��Wrap=�6N   
���������5  I   �� *	Message+5  b   �� 5  2   $��2��6<   $��Önerilen Aksiyonlar:��6I   $��SemiBold"�5  <   
�������$��0,0,0,5q�6e   y�5  <   �� *
HasActions+# [  �� 5  2   $��3��6?   ��6L   �� *	Actions+y�5  ?   �� *
HasActions+# [  ��5  6   �� 5  :   .R�5  >   �� $��10,2,0,2q�5  H   
�������$��12(�6m   ��5  H   �� *.+$! • {0}��5  m   5  ;   5   3   �� 5#  2   $��4��6<   $��0,10,0,0q�5$  <   $��10(�6N   
�������$��Right=�6o   ��5#  I   �� *	CreatedAt+$(! "Oluşturulma: {0:dd.MM.yyyy HH:mm}��5$  o   5%  /   5&  +   5'  '   5)     5*     � 5-     -L   5        $3 panelEmptyRecommendations��5-  ,   $��3��6   $y�Visible=�6O   .+�5.     �� $��Center=�6)   $��Center=�6F   .+�5/  "   �� $
��🎯��6,   $��64(�66   $��Center=�6D   : J� Opacity$	: 0.3Q�6a   �� 50  "   $��Hasta Seçin��6,   $��18(�6?   $��SemiBold"�6M   $��Center=�51  ,   $��0,10,0,0q�6I   ���#9996[   �� 52  "   $>��8Sol taraftan hasta seçerek önerilerini görüntüleyin��6,   $��Center=�53  ,   $��0,5,0,0q�6I   ���#9996Z   54     55     56     57     �� 5:     $��Profil��6   .��5;     � $��20q�6    5<     �� 5=     $
��Auto �6,   �� 5>     $��* �6,   .+�5A     �� $��0��6$   $��Hesap Bilgilerim��61   #��[ 6I   �� 5C     $��1��6'   $
��Auto=�64   .��5D     �� $	��500��6)   $
��Left=�65   .+�5F  "   �� ���#F5F5F5$ 20q�6>   $��0,0,0,20q�6K   $ 5��6]   ._�5G  &   �  5H  *   �� 5I  .   $	��150 �6?   �� 5J  .   $��* �6?    5L  *   �� 5M  .   $
��Auto �6<   �� 5N  .   $
��Auto �6<   �� 5O  .   $
��Auto �6<   �� 5P  .   $
��Auto �6<   �� 5Q  .   $
��Auto �6<   .+�5T  *   �� $��0��64   $��0��6A   $��TC Kimlik No:��6Q   $��SemiBold"�6f   �� 5U  *   -M   5        $
 txtDoctorProfileTcId��5U  Q   $��0��64   $��1��6A   $��12345678901��6o   �� 5W  *   $��1��64   $��0��6A   $��	Ad Soyad:��6Q   $��SemiBold"�6b   $��0,10,0,0q�6x   �� 5X  *   -N   5        $
 txtDoctorProfileName��5X  Q   $��1��64   $��1��6A   $��Ad Soyad��6o   $��0,10,0,0q�6   �� 5Z  *   $��2��64   $��0��6A   $��Doğum Tarihi:��6Q   $��SemiBold"�6f   $��0,10,0,0q�6|   �� 5[  *   -O   5        $
 txtDoctorProfileBirthDate��5[  Q   $��2��64   $��1��6A   $��
01.01.1980��6t   $��0,10,0,0q�6�   �� 5]  *   $��3��64   $��0��6A   $��	Cinsiyet:��6Q   $��SemiBold"�6b   $��0,10,0,0q�6x   �� 5^  *   -P   5        $
 txtDoctorProfileGender��5^  Q   $��3��64   $��1��6A   $��Erkek��6q   $��0,10,0,0q�6~   �� 5`  *   $��4��64   $��0��6A   $��E-posta:��6Q   $��SemiBold"�6a   $��0,10,0,0q�6w   �� 5a  *   -Q   5        $
 txtDoctorProfileEmail��5a  Q   $��4��64   $��1��6A   $��doktor@example.com��6p   $��0,10,0,0q�6�   5b  '   5c  #   �� 5f  "   $��Profil Resmi��6,   #��[ 6@   �� 5h  "   ���#F5F5F5$ 20q�6>   $��	0,10,0,20q�6K   $ 5��6^   ._�5i  &   �  5j  *   �� 5k  .   $	��150 �6?   �� 5l  .   $��* �6?    5n  *   �� 5o  .   $
��Auto �6<   �� 5p  .   $
��Auto �6<   .+�5t  *   �� $��0��61   $��0��6>   $��2��6N   $	��150��6b   $	��150��6n   
�������$ 75��5u  H   $��0,0,0,15q�6Z   $��Center=�6l   ._�5v  .   �� -R   5        ; ��Name$; imgDoctorProfilePicture��5v  4   $��UniformToFill=�6U   5w  +   �� 5z  *   -S   5        $��1��5z  1   $��0��6>   $��2��6N   $��Profil Resmi Yükle��6b   $��Center=�5{  1   #��[ 6N   5|  1   5}  '   5~  #   �� 5�  "   $��Şifre Değiştir��6,   #��[ 6B   � 5�  "   $��0,10,0,0q�6'    5�  &   �� 5�  *   $��* �6;   �� 5�  *   $��* �6;    5�  &   �� 5�  *   $
��Auto �68   �� 5�  *   $
��Auto �68   �� 5�  *   $
��Auto �68   .+�5�  &   �� $��0��60   $��0��6=   $��Mevcut Şifre:��6M   $��0,5,10,5q�6b   G� 5�  &   -T   5        < G�Name$< txtDoctorCurrentPassword��5�  O   $��0��62   $��1��6?   $��10,5,0,5q�6q   �� 5�  &   $��1��60   $��0��6=   $��Yeni Şifre:��6M   $��0,5,10,5q�6`   G� 5�  &   -U   5        $< txtDoctorNewPassword��5�  O   $��1��62   $��1��6?   $��10,5,0,5q�6m   �� 5�  &   $��2��60   $��0��6=   $��Yeni Şifre (Tekrar):��6M   $��0,5,10,5q�6i   G� 5�  &   -V   5        $< txtDoctorConfirmPassword��5�  O   $��2��62   $��1��6?   $��10,5,0,5q�6q   5�  #   �� 5�  "   -W   5        $��Şifreyi Değiştir��5�  )   #��[ 6D   $��Right=�5�  (   $��0,10,0,0q�6D   6V   5�     5�     5�     5�     5�     5�     5�     5�     