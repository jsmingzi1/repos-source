   M S B A M L   `   `   `  ����C  ?UiStudio, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:UiStudioUiStudio  5   	       UiStudio.NewFileWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35` \RibbonControlsLibrary, Version=3.5.41019.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:UiStudio     5   	     � Title$  NewFileWindow��5   	   $	��162��6   $	��400��6,    � WindowStyle$ 
ToolWindow=�68   $��2q�6Q    � WindowStartupLocation$ CenterScreen=�5	   	   #��� ��6.   .��5
      �  � RowDefinitions 5   
   �� 5      $	��100 �6   �� 5      $��40 �6   .+�5   
   �� $��0��6    �� Orientation$ Vertical=�6"   .+�5      �� -   5        $��m_filenamelabel��5   E   $��Please input new filename��6   $	��0,0q�68   $
��Bold"�6\   �� 5      -   5        $��
m_filename��5   -   $�� ��6   $
��0, 0q�6   #��� ��6?   �� 5      $��Under below path��6   $	��0,0q�60   $
��Bold"�6=   �� 5      -   5        $��m_foldername��5   ?   $�� ��6   $
��0, 0q�6    �� 
IsReadOnly . #��� ��6S   5      ]� 5   
   $��1��6   .+�5      �� .+�5      �  � ColumnDefinitions 5      �� 5      �� 5      �� 5      �� 5      �� 5      .+�5!      �� -   5        $��1��5!      $��OK��6-   $
��Bold"�6P   #��� ��6b   �� 5"      -   5        $��3��5"      $��Cancel��6-   $
��Bold"�6U   #��� ��6g   5#      5%      5&      5'      5(      