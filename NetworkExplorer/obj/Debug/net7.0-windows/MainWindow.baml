   M S B A M L   `   `   `  ����J  FNetworkExplorer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:NetworkExplorerNetworkExplorer  5   	        NetworkExplorer.MainWindow   5   	   W SPresentationCore, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   /localclr-namespace:NetworkExplorer     5   	   -   5          � Title$  
MainWindow��5   	   $	��500��6   $	��850��6)    � 
ResizeMode$ CanMinimize=�65    � 	Resources 5	         _nodesViewSource  _disabledNodesViewSource��%:   &	        &	      �� 5
   
   6   �� 5   
   6   5      .��5      � .+�5   
   &   System.Windows.Controls.DataGrid  $��10,48,10,257q�6U      EnableRowVirtualization$ False��6k      AutoGenerateColumns$ False��6�   ��6   ��  �� Source# [     Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn   Header$ IP��5   %      
IsReadOnly$
 True��5   %   1  +System.Windows.Controls.DataGridBoundColumn	   Binding	 5   %   �� *
IPAdress+5   %     5      $	 MAC��6=   
   Width5  /System.Windows.Controls.DataGridLengthConverter$
 * 5   )   $
 True��63   	 5   %   �� *MAC+5   3     5      $
 Name��6>   $
 * 5   )   $
 True��63   	 5   %   �� *Name+5   3   4  .System.Windows.Controls.DataGridTemplateColumn  5      -   5        EditButtons��$ Details��5   >      
Visibility$ 	Collapsed=�6O      CellTemplate 5      �� 5      .R�5      �� -   5        $
��Edit��5   %   64   5      5      5      �� 5    
   -   5         ��Name$ RefreshButton��5       $��Refresh��6(   $
��Left=�6:   $��
670,10,0,0q�6U   $	��Top=�6i   $��33��6�   $	��141��6�   6�   �� 5!   
   -   5        $ AdminButton��5!      $��
Admin mode��6&   $
��Left=�6;   $��
10,425,0,0q�6V   $	��Top=�6j   $	��150��6�   $��33��6�   6�     5"   
   -   5          Name$ DisabledNodesTable��5"      $��10,232,10,64q�6U   $y�	Collapsed=�6k   ��6/   �� *DisabledNodes+ 5#        5$      $ IP��5%   %   $
 True��5&   %   	 5$   %   �� *
IPAdress+5&   %     5'      $	 MAC��6=   $
 * 5(   )   $
 True��63   	 5'   %   �� *MAC+5(   3     5)      -   5        EnableButtons��$ Enable��5)   @    5*      �� 5+      .R�5,      �� -   5        $��Enable��5,   %   66   5-      5/      51      52      53      