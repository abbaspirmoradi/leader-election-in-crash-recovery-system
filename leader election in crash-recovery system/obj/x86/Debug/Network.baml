   M S B A M L   `   `   `  ����c  _leader election in crash-recovery system, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullc6clr-namespace:leader_election_in_crash_recovery_system(leader_election_in_crash_recovery_system  5   	   6    0leader_election_in_crash_recovery_system.Network   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   Hlocal6clr-namespace:leader_election_in_crash_recovery_system     5   	     � Title$  Network Creator��5   	   $	��200��5   	   $	��200��5   	   $	��600��5   	   $	��650��5	   	   
������� � 	Resources 5         MyGridStyle �� 
TargetType=   7leader_election_in_crash_recovery_system.DataGridPCtrls �� Property �� Value��%�  &	        �� 5   
   "  6$   .�5      ��  �@ GridBackColor
 �����6.   �� 5       �@ GridLinesColor
 �����6/   �� 5       �@ CanUserResizeColumns . 65   �� 5       �@ CanUserSortColumns . 63   �� 5       � ��$	 Top=�62   �� 5       � ��$	 650��6*   �� 5       � ��$ 0��6*   �� 5       � ��$ RightToLeft=�6.   5      5      .��5      �  � RowDefinitions 5   
   �� 5      $��20 �6   �� 5      $��60 �6   �� 5      $��* �6   �� 5      $��60 �6   �� 5      $��60 �6    � ColumnDefinitions 5!   
   �� 5"      $��40 �6   �� 5#      $��* �6   �� 5$      $��40 �6   .+�5&   
   �� -   5        $��border0��5&      $��1��5'      $��1��5(      $��40��5)      $��Center=�5*      
�������
���   �$��2q�5-       �� CornerRadius$ 6��5.       �� Padding$ 
8,10,10,10q�5/      $��Visible=�50      ._�51      �� -   5        $��TXTname��51      52      �� 53   
   -   5        $��border��53      $��2��54      $��1��55      $��Center=�56      
�������
�������$��5q�59      $ 6��5:      $ 
8,10,10,10q�5;      $��Visible=�5<      ._�5=        -   5        	  Name$	 TablesDataGrid��5=   #   #��[  6;   5>      �� 5@   
   $��4��6   $��1��5A      $��Center=�5B      $��LeftToRight=�5C      
 �� Orientation$
 
Horizontal=�5D      .+�5F      �� -   5        $��Confirm��5F      $	��100��5G      $��30��5H      
�������.��5K      �� $
 
Horizontal=�6   .+�5L      �� $��40��6   $��0q�6'   .��63   Done!6:   �� 5M      $P��J/leader%20election%20in%20crash-recovery%20system;component/Icons/Done.png��6   5N      5O      �� 5Q      $	��100��6   �� 5R      -   5        $��Cancel��5R      $	��100��5S      $��30��5T      
�������.��5W      �� $
 
Horizontal=�6   .+�5X      �� $��40��6   $��0q�6'   .��63   Cancel6;   �� 5Y      $R��L/leader%20election%20in%20crash-recovery%20system;component/Icons/Cancel.png��6   5Z      5\      5^      �� 5_   
   -   5        $��border4��5_      $��3��5`      $��1��5a      $��45��5b      $��Center=�5c      
�������
���   �$��2q�5f      $ 6��5g      $ 
8,10,10,10q�5h      $��Visible=�5i      ._�5j      �� $��35��6   $��Center=�5k      $
 
Horizontal=�5l      .+�5m      �� $	��140��6   $��Center=�6(   NetWork Runnig Time(s) :6]   �� 5n      -   5        $��
txtNetTime��5n      $	��100��5o      $��20��5p      5q      5r      5t      5u      