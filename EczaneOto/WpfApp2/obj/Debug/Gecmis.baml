   M S B A M L   `   `   `  ����B  >WpfApp2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:WpfApp2WpfApp2  5   	       WpfApp2.Gecmis   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   'localclr-namespace:WpfApp2     5   	     � Title$  Geçmiş Reçete Sistemi��5   	   $	��900��6'   $
��1600��64    � WindowStyle$
 None=�5	   	    � 
ResizeMode$ 	CanResize=�6   
������  � AllowsTransparency .  � WindowStartupLocation$ CenterScreen=�5   	    � 	Resources 5         MainGradient �� 
StartPoint �� EndPoint � Color	 � Offset  SidebarGradient  CardGradient  StatsGradient  ModernButtonStyle
 �� 
TargetType �� Property �� Value �� 
TargetType ��Name �� CornerRadius J� Effect3  -System.Windows.Media.Effects.DropShadowEffect   ShadowDepth   
BlurRadius   Opacity   Color �� Triggers P� Property P� Value �� 
TargetName  ModernMenuStyle �� Triggers  ModernCardStyle  StatsCardStyle  HeaderTextStyle 	 SmallHeaderTextStyle 
 MenuTextStyle  StatsNumberStyle  StatsLabelStyle��%�  &	        &	 �     &	 (    &	 �    &	 P    &	 �    &	 $    &	 G    7  &	 d	    &		 &
    &	
 �
    &	 �    &	 d    �� 5   
   $	 0,0&�63   $	 1,1&�6D   .��5      � $ #667eea��6   $	 0Q�6+   � 5      $ #764ba2��6   $	 1Q�6+   5      �� 5   
   $	 0,0&�66   $	 0,1&�6G   .��5      � $ #2c3e50��6   $	 0Q�6+   � 5      $ #34495e��6   $	 1Q�6+   5      �� 5   
   $	 0,0&�63   $	 0,1&�6D   .��5      � $ #ffffff��6   $	 0Q�6+   � 5      $ #f8f9fa��6   $	 1Q�6+   5      �� 5   
   $	 0,0&�64   $	 1,1&�6E   .��5       � $ #FF6B73��6   $	 0Q�6+   � 5!      $ #FF8E8E��6   $	 1Q�6+   5"      �� 5%   
   "
 ��6*   .�5&      ��  � �� �#667eea6+   �� 5'       � ��
 �����6+   �� 5(       � ��$ 14(�6)   �� 5)       � ��$ SemiBold"�6+   �� 5*       � ��$ 20,12q�6(   �� 5+       � ��$ 8q�6'   �� 5,       � ��$ 0q�60   �� 5-       �@�Cursor$
 Hand��6'   �� 5.       � �� 5/      �� 50      " ��6&   .R�51      �� $ border��6!   #��z��61   $ 8��52   !   $��0q�62    53        54   "   $ 2Q�63   $ 8Q�6C   $	 0.3Q�6R   $ Black��6`   ._�56      �� $��Center=�6/   $��Center=�6L   57       58      P� 59       �@J�IsMouseOver . . �5:   "   �� $ border��6)    � �� �#5a67d86S   5;      P� 5<       � �� . . �5=   "   �� $ border��6)    � �� �#4c51bf6S   5>      5@      5B      5C      �� 5F   
   "
 ��6(   .�5G      ��  � ��
 ���� 6+   �� 5H       � ��$	 0,8q�6'   �� 5I       �@��Padding$ 15,12q�6(   �� 5J       �@��CornerRadius$ 10��6-   �� 5K       �@�Cursor$
 Hand��6'    5L      P� 5M       �@J�IsMouseOver . . �5N      ��  � �� �#4a556863   5O      5Q      �� 5T   
   "
 ��6(   .�5U      ��  � ��
 �����6+   �� 5V       �@��CornerRadius$ 15��6-   �� 5W       �@��Padding$ 20q�6(   �� 5X       �@J�Effect 5Y        5Z      $ 3Q�6'   $ 15Q�67   $	 0.2Q�6G   $ Black��6U   5\      5]      �� 5`   
   "
 ��6'   .�5a      ��  � ��8   6+   �� 5b       �@��CornerRadius$ 15��6-   �� 5c       �@��Padding$ 20q�6(   �� 5d       �@J�Effect 5e        5f      $ 3Q�6'   $ 15Q�67   $	 0.2Q�6G   $ Black��6U   5h      5i      �� 5l   
   "
 ��6(   .�5m      ��  � ��$ 20(�6)   �� 5n       � ��$
 Bold"�6+   �� 5o       � �� �#2d37486+   �� 5p       � ��$ 0,0,0,15q�6'   5q      �� 5t   
   "
 ��6-   .�5u      ��  � ��$ 14(�6)   �� 5v       � ��$
 Bold"�6+   �� 5w       � �� �#2d37486+   �� 5x       � ��$ 0,0,0,10q�6'   5y      �� 5|   
   "
 ��6&   .�5}      ��  � ��
 �����6+   �� 5~       � ��$ 16(�6)   �� 5       � ��$ SemiBold"�6+   �� 5�       � ��$ Center=�62   5�      �� 5�   
   "
 ��6)   .�5�      ��  � ��$ 32(�6)   �� 5�       � ��$
 Bold"�6+   �� 5�       � ��
 �����6+   �� 5�       � ��$ Center=�64   5�      �� 5�   
   "
 ��6(   .�5�      ��  � ��$ 14(�6)   �� 5�       � ��
 �����6+   �� 5�       � ��$ Center=�64   �� 5�       �@J�Opacity$	 0.9Q�6(   5�      5�      .��5�      � .+�5�   
   �� #��[  6    �� RadiusX$ 20��69    �� RadiusY$ 20��6F   � 5�   
   $��20q�6    � RowDefinitions 5�      �� 5�      $��80 �6    �� 5�      $��* �6    .+�5�      �� $��0��6   #��[ 6"   $��0,0,0,15q�6K   ._�5�      � .+�5�      ��  �� Orientation$ 
Horizontal=�6!   $��0,-18,0,-19q�6:   .+�5�      A� $��50��6"   $��50��6-   ���	#FF38B2AC$��0,0,15,0q�6J    5�        5�   "   $ 2Q�63   $ 8Q�6C   $	 0.3Q�6R   5�      �� 5�      $��Center=�6%   .+�5�      �� $#��📄 Geçmiş Reçete Sistemi��6(   $��24(�6H   $
��Bold"�6V   ���#2d37486h   �� 5�      $)��#Reçete Geçmişi ve Fiş Yönetimi��6(   $��14(�6N   ���#7180966\   5�      5�      �� 5�      $ 
Horizontal=�6!   $��Right=�6:   $��Center=�6V   .+�5�      �� -   5        $
��🗕��5�   !   $��40��6.   $��30��69   
������ $��0q�6^   ���#718096$��16(�6�    � Cursor$
 Hand��6�   6�   �� 5�      -   5        $	��❌��5�   !   $��40��6-   $��30��68   
������ $��0q�6]   ���#e53e3e$��14(�6�   $
 Hand��6�   6�   5�      5�      5�      � 5�      $��1��6    � ColumnDefinitions 5�      �� 5�      $	��300 �6'   �� 5�      $��* �6'   .+�5�      �� $��0��6   #��[ 6)   $ 15��5�      $��0,0,15,0q�6+    5�        5�      $ 3Q�6+   $ 15Q�6;   $	 0.3Q�6K   $ Black��6Y   ._�5�      �� $
��Auto=�6#   .��5�      �� $��20q�6%   .+�5�      �� ���	#FFFF2020$ 15��6<     �� Padding$  20q�6N   $��0,0,0,30q�6[   ._�5�   "   �� $��80��6(   $��80��64   $��/eczane.png��6?   $��Center=�6T   $��Center=�6q   5�      �� 5�      -   5        #��[ 5�   %   ._�5�   "   �� $ 
Horizontal=�6-   .+�5�   &   �� $��35��6-   $��35��68   ���#667eea$ 8��6Y   $��0,0,15,0q�6j   ._�5�   *   �� $
��💊��64   $��18(�6>   $��Center=�6L   $��Center=�6i    5�   *     5�   .   $ 1Q�6?   $ 4Q�6O   $	 0.3Q�6^   5�   '   �� 5�   &   $��Reçete Oluştur��60   #��[
 6F   5�   #   5�      �� 5�      -   5        #��[ 5�   %   ._�5�   "   �� $ 
Horizontal=�6-   .+�5�   &   �� $��35��6-   $��35��68   ���#48bb78$ 8��6Y   $��0,0,15,0q�6j   ._�5�   *   �� $
��👥��64   $��18(�6>   $��Center=�6L   $��Center=�6i    5�   *     5�   .   $ 1Q�6?   $ 4Q�6O   $	 0.3Q�6^   5�   '   �� 5�   &   $��Hastalar��60   #��[
 6@   5�   #   5�      �� 5�      -   5        #��[ 5�   %   ._�5�   "   �� $ 
Horizontal=�6-   .+�5�   &   �� $��35��6-   $��35��68   ���#ed8936$ 8��6Y   $��0,0,15,0q�6j   ._�5�   *   �� $
��📋��64   $��18(�6>   $��Center=�6L   $��Center=�6i    5�   *     5�   .   $ 1Q�6?   $ 4Q�6O   $	 0.3Q�6^   5�   '   �� 5�   &   $��Reçete��60   #��[
 6>   5�   #   5�      �� 5�      -   5        #��[ 5�   %   ._�5�   "   �� $ 
Horizontal=�6-   .+�5�   &   �� $��35��6-   $��35��68   ���#9f7aea$ 8��6Y   $��0,0,15,0q�6j   ._�5�   *   �� $
��📦��64   $��18(�6>   $��Center=�6L   $��Center=�6i    5�   *     5�   .   $ 1Q�6?   $ 4Q�6O   $	 0.3Q�6^   5�   '   �� 5�   &   $
��Stok��60   #��[
 6<   5�   #   5�      �� 5�      #��[ 6%   ���#4a5568._�5�   "   �� $ 
Horizontal=�6-   .+�5�   &   �� $��35��6-   $��35��68   ���#38b2ac$ 8��6Y   $��0,0,15,0q�6j   ._�5�   *   �� $
��📄��64   $��18(�6>   $��Center=�6L   $��Center=�6i    5�   *     5�   .   $ 1Q�6?   $ 4Q�6O   $	 0.3Q�6^   5  '   �� 5  &   $��Geçmiş Reçete��60   #��[
 6E   5  #   5     �� 5     #��[ 6%   ._�5  "   �� $ 
Horizontal=�6-   .+�5  &   �� $��35��6-   $��35��68   ���#e53e3e$ 8��6Y   $��0,0,15,0q�6j   ._�5  *   �� $
��📊��64   $��18(�6>   $��Center=�6L   $��Center=�6i    5	  *     5
  .   $ 1Q�6?   $ 4Q�6O   $	 0.3Q�6^   5  '   �� 5  &   -   5        $��	Grafikler��5  0   #��[
 6A   6h   5  #   5     5     5     5     � 5     $��1��6    5     �� 5     $
��Auto �6(   �� 5     $	��140 �6(   �� 5     $��* �6(   .+�5     �� $��0��6   #��[ 6*   $��0,0,0,15q�6S   ._�5     �  5     �� 5   "   $��* �63   �� 5!  "   $��* �63   .+�5%     �� $��0��6)   $ 
Horizontal=�69   $
��Left=�6R   $��Center=�6m   .+�5&  "   �� -   5        $��GecmisGoster��5&  )   $��📋 Fişleri Göster��6=   #��[ 6Y   ���#38b2ac$��40��5'  C   $	��140��6O   �� 5(  "   -	   5        $��IstatistikGoster��5(  )   $$��📊 İstatistikleri Güncelle��6A   #��[ 6f   ���#FF6B73$��40��5)  G   $	��165��6S   5*     �� 5-     $��1��6)   $ 
Horizontal=�69   $��Right=�6R   $��Center=�6n   .+�5.  "   �� ���#e53e3e$ 10��6>   $��5q�6P    5/  &     50  *   $ 2Q�6;   $ 8Q�6K   $	 0.3Q�6Z   $ Black��6h   ._�52  &   �� -
   5        $��
PdfCiktiAl��52  -   $��📄 PDF Çıktı��6?   #��[ 6V   
������ $��40��53  I   $	��120��6U   $��0q�54     
�������6+   55  #   �� 57  "   ���#22c55e$ 10��6>   $��5q�6P    58  &     59  *   $ 2Q�6;   $ 8Q�6K   $	 0.3Q�6Z   $ Black��6h   ._�5;  &   �� -   5        $��ExcelCiktiAl��5;  -   $��📊 Excel Çıktı��6A   #��[ 6Z   
������ $��40��5<  K   $	��120��6W   $��0q�5=     
�������6+   5>  #   5?     5@     5A     � 5D     $��1��6   $��0,0,0,15q�6(    5E     �� 5F     $��* �6/   �� 5G     $��* �6/   �� 5H     $��* �6/   �� 5I     $��* �6/   .+�5M     �� $��0��6!   #��[ 61   $��0,0,8,0q�6Y   ._�5N     �� .+�5O  "   �� $��☀️��6,   $��24(�66   $��Center=�6D   $��0,0,0,8q�6a   �� 5P  "   -   5        $��BugunkuSatis��5P  ,   $��0��6@   #��[ 6I   �� 5Q  "   $��Bugün��6,   #��[ 69   5R     5S     �� 5V     $��1��6!   #��[ 61   $��8,0,8,0q�6Y   ��5W     �� 5X  "   $	 0,0&�66   $	 1,1&�6G   .��5Y  &   � $ #667eea��63   $	 0Q�6C   � 5Z  &   $ #764ba2��63   $	 1Q�6C   5[  #   ._�5]     �� $  ��6)   .+�5^  "   �� $
��📅��6,   $��24(�66   $��Center=�6D   $��0,0,0,8q�6a   �� 5_  "   -   5        $��HaftalikSatis��5_  ,   $��0��6A   #��[ 6J   �� 5`  "   $��Bu Hafta��6,   #��[ 6<   5a     5b     �� 5e     $��2��6!   #��[ 61   $��8,0,8,0q�6Y   ��5f     �� 5g  "   $	 0,0&�66   $	 1,1&�6G   .��5h  &   � $ #48bb78��63   $	 0Q�6C   � 5i  &   $ #38a169��63   $	 1Q�6C   5j  #   ._�5l     �� .+�5m  "   �� $��🗓️��6,   $��24(�67   $��Center=�6E   $��0,0,0,8q�6b   �� 5n  "   -   5        $��
AylikSatis��5n  ,   $��0��6>   #��[ 6G   �� 5o  "   $��Bu Ay��6,   #��[ 69   5p     5q     �� 5t     $��3��6!   #��[ 61   $��8,0,0,0q�6Y   ��5u     �� 5v  "   $	 0,0&�66   $	 1,1&�6G   .��5w  &   � $ #ed8936��63   $	 0Q�6C   � 5x  &   $ #dd6b20��63   $	 1Q�6C   5y  #   ._�5{     �� .+�5|  "   �� $
��📊��6,   $��24(�66   $��Center=�6D   $��0,0,0,8q�6a   �� 5}  "   -   5        $��ToplamSatis��5}  ,   $��0��6?   #��[ 6H   �� 5~  "   $��Toplam��6,   #��[ 6:   5     5�     5�     � 5�     $��2��6    5�     �� 5�     $��2* �6/   �� 5�     $��* �6/   .+�5�     �� $��0��6!   #��[ 61   $��0,0,8,0q�6Z   ._�5�     �  5�  "   �� 5�  &   $
��Auto �64   �� 5�  &   $��* �64   .+�5�  "   �� $��0��6,   $#��📄 Geçmiş Reçete Listesi��69   #��[ 6Y   $��Center=�6�   &   System.Windows.Controls.DataGrid  5�  "   -   5        $��dataGridReceteler��5�  8   $��1��6+   !   AutoGenerateColumns! .  
�������$��0q�5�  <   "   GridLinesVisibility$" 
Horizontal=�6P   #   HorizontalGridLinesBrush# �#E0E0E0$   RowBackground
$ �����%   AlternatingRowBackground% �#F8F9FA&   HeadersVisibility$& Column=�5�  L   '   ColumnHeaderStyle' 5�  &   �� 5�  *   =  7System.Windows.Controls.Primitives.DataGridColumnHeader"
  60   .�5�  .   ��  � �� �#38b2ac6K   �� 5�  .    � ��
 �����6K   �� 5�  .    � ��$ SemiBold"�6K   �� 5�  .    � ��$
 12,8q�6H   �� 5�  .    � ��$ 0q�6P   �� 5�  .    � ��$ Center=�6[   5�  +   (   Columns( 5�  &   0  *System.Windows.Controls.DataGridTextColumn  5�  *   ,  &System.Windows.Controls.DataGridColumn)   Header$) 
Reçete No��6=   *   Width5  /System.Windows.Controls.DataGridLengthConverter$* * 6m   1  +System.Windows.Controls.DataGridBoundColumn+   Binding+ 6P   �� *
ReceteID+6m     5�  *   $) T.C. No��6=   $* * 6e   + 6N   �� *ID+6e     5�  *   $) Fiyat��6=   $* * 6f   + 6L   �� *Fiyat+6f     5�  *   $) Tarih��6=   $* * 6f   + 6L   �� *Tarih+6f   5�  #   5�     5�     � 5�     $��1��6   $��8,0,0,0q�6/    5�     �� 5�  "   $��* �60   �� 5�  "   $��15 �60   �� 5�  "   $��* �60   .+�5�     �� $��0��6%   #��[ 62   ._�5�  "   �  5�  &   �� 5�  *   $
��Auto �68   �� 5�  *   $��* �68   .+�5�  &   �� $��0��60   $��📈 Günlük Satış��6=   #��[	 6T   $��Center=�6�     5�  &   -   5        $��dataGridGunlukSatis��5�  <   $��1��6/   ! .  
�������$��0q�5�  @   $" 
Horizontal=�6T   # �#E0E0E0
$ �����% �#F8F9FA$& Column=�5�  P   ' 5�  *   �� 5�  .   "
  64   .�5�  2   ��  � �� �#FF6B736O   �� 5�  2    � ��
 �����6O   �� 5�  2    � ��$ SemiBold"�6O   �� 5�  2    � ��$	 6,4q�6L   �� 5�  2    � ��$ 0q�6T   �� 5�  2    � ��$ Center=�6_   �� 5�  2    � ��$ 11(�6M   5�  /   ( 5�  *     5�  .   $) Tarih��6A   $* 60 6j   + 6P   �� *Tarih+6j     5�  .   $
) Adet��6A   $* 40 6n   + 6O   �� *
SatisAdeti+6n     5�  .   $) Toplam��6A   $* 60 6q   + 6Q   �� *ToplamTutar+6q   5�  '   5�  #   5�     � 5�     $��2��6#    5�  "   �� 5�  &   $��* �67   �� 5�  &   $��8 �67   �� 5�  &   $��* �67   .+�5�  "   �� $��0��6)   #��[ 69   ._�5�  &   �  5�  *   �� 5�  .   $
��Auto �6<   �� 5�  .   $��* �6<   .+�5�  *   �� $��0��64   $��🗓️ Aylık��6A   #��[	 6R   $��Center=�6�     5�  *   -   5        $��dataGridAylikSatis��5�  @   $��1��63   ! .  
�������$��0q�5�  D   $" 
Horizontal=�6X   # �#E0E0E0
$ �����% �#F8F9FA$& Column=�5�  T   ' 5�  .   �� 5�  2   "
  68   .�5�  6   ��  � �� �#48bb786S   �� 5�  6    � ��
 �����6S   �� 5�  6    � ��$ SemiBold"�6S   �� 5�  6    � ��$	 4,3q�6P   �� 5�  6    � ��$ 0q�6X   �� 5�  6    � ��$ Center=�6c   �� 5�  6    � ��$ 10(�6Q   5�  3   ( 5�  .     5�  2   $) Ay��6E   $* 65 6h   + 6Q   �� *Ay+6h     5�  2   $
) Adet��6E   $* 30 6r   + 6S   �� *
SatisAdeti+6r     5�  2   $) Tutar��6E   $* 45 6t   + 6T   �� *ToplamTutar+6t   5�  +   5�  '   5�  #   �� 5  "   $��2��6)   #��[ 69   ._�5  &   �  5  *   �� 5  .   $
��Auto �6<   �� 5  .   $��* �6<   .+�5  *   �� $��0��64   $��📅 Yıllık��6A   #��[	 6R   $��Center=�6�     5
  *   -   5        $��dataGridYillikSatis��5
  @   $��1��63   ! .  
�������$��0q�5  D   $" 
Horizontal=�6X   # �#E0E0E0
$ �����% �#F8F9FA$& Column=�5  T   ' 5  .   �� 5  2   "
  68   .�5  6   ��  � �� �#ed89366S   �� 5  6    � ��
 �����6S   �� 5  6    � ��$ SemiBold"�6S   �� 5  6    � ��$	 4,3q�6P   �� 5  6    � ��$ 0q�6X   �� 5  6    � ��$ Center=�6c   �� 5  6    � ��$ 10(�6Q   5  3   ( 5  .     5  2   $
) Yıl��6E   $* 35 6j   + 6R   �� *Yil+6j     5  2   $
) Adet��6E   $* 30 6r   + 6S   �� *
SatisAdeti+6r     5  2   $) Tutar��6E   $* 45 6t   + 6T   �� *ToplamTutar+6t   5  +   5  '   5   #   5!     5"     5#     5$     5%     5&     5'     5(     