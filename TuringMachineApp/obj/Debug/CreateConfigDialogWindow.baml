   M S B A M L   `   `   `  ����K  GTuringMachineApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:TuringMachineAppTuringMachineApp  5   	   O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem 5   	   ^ ZTuringMachineViewModelComponentsLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null�^clr-namespace:TuringMachineViewModelComponentsLib;assembly=TuringMachineViewModelComponentsLib#TuringMachineViewModelComponentsLib 5   	   /    )TuringMachineApp.CreateConfigDialogWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   0localclr-namespace:TuringMachineApp     5   	   6sys&clr-namespace:System;assembly=mscorlib     5   	   svmcmpnts^clr-namespace:TuringMachineViewModelComponentsLib;assembly=TuringMachineViewModelComponentsLib     5   	     � Title$-  'Создать конфигурацию��5
   	   $	��300��6&   $	��400��63    � 	Resources 5      G  ATuringMachineViewModelComponentsLib.ComboBoxItemToObjectConverter   cbiToObjConverter  notSelected �� 
TargetType �� Triggers P� Property P� Value �� Property �� Value �� RelativeSource	 �� Path��%�  &	        &	      '�� <       5   
   61   ��5   
   Не выбран��64   �� 5   
   " ��6    5      P� 5       �@=�HasError . . �5      ��  �@�ToolTip 5      ��  �� *Self+$)	 #(Validation.Errors)[0].ErrorContent�5      P� 5       �@=�HasError .  . �5      ��  �@�ToolTip 5      ��  �� *Self+$		 Tag�5      5      5!      .��5#      � 
 � RowDefinitions
 5%   
   �� 5&      $��1* �6   �� 5'      $��1* �6   �� 5(      $��1* �6   �� 5)      $��1* �6   �� 5*      $��1* �6   �� 5+      $
��1.5* �6   �� 5,      $
��1.5* �6    � ColumnDefinitions 5/   
   �� 50      $��1* �6   �� 51      $
��2.5* �6   .+�54   
   �� $��Алфавит:��6   $��0��55      $��0��6   �� 56   
   $��0��6   $��1��6   
 � Tag$� �Введите строку символов. '_' должно отсутствовать. Символы не должны повторяться.��57      .��6j   ��58      �� 59      $	 Alphabet�6    �� Mode$ TwoWay=�6*    �� UpdateSourceTrigger$ PropertyChanged=�68    �� ValidationRules 5:      B  <TuringMachineViewModelComponentsLib.TMAlphabetValidationRule  5;      5=      .��5>      5?      �� 5A   
   $��Размер ленты:��6   $��1��5B      $��0��6   �� 5C   
   $��1��6   $��1��6   $B <Введите целое натуральное число.��5D      .��69   ��5E      �� 5F      $	 
MemorySize�6   $ TwoWay=�6,   $ PropertyChanged=�6:    5G      E  ?TuringMachineViewModelComponentsLib.NaturalNumberValidationRule  5H      5J      .��5K      5L      �� 5N   
   $$��Источник данных:��6   $��2��5O      $��0��6   �� 5P   
   $��2��6   $��1��6    $��25��5Q      $	��100��6   $
��Left=�5R      $��5,0,0,0q�5S       � ToolTip$( "Выберите источник.��5[       �� Text 5T      �� $	 InitialDataSource�$ OneWay=� �� TargetNullValue# [  �� SelectedItem 5W      �� $	 InitialDataSource�$ OneWayToSource=�$ PropertyChanged=� �� 	Converter# [  .@�5\      �� .��6   
Текст6"   �� 5]      .��6   
Файл6!   5^      �� 5`   
   $&�� Начальные данные:��6   $��3��5a      $��0��6   �� 5b   
   $��3��6   $��1��6   $> 8Введите текст или путь к файлу.��5c      .��68   ��5d      �� 5e      $	 InitialData�6   $ TwoWay=�6-   $ PropertyChanged=�6;    5f      5i      .��5j      5k      �� 5m   
   $G��AСмещение начальных данных на ленте:��6   $��WrapWithOverflow=�5n      $��4��5o      $��0��6!   �� 5p   
   $��4��6   $��1��6   $B <Введите целое натуральное число.��5q      .��69   ��5r      �� 5s      $	 InitialDataOffset�6   $ TwoWay=�63   $ PropertyChanged=�6A    5t        5u      5w      .��5x      5y      �� 5{   
   $K��EСмещение головки от начальных данных:��6   $��WrapWithOverflow=�5|      $��5��5}      $��0��6!   �� 5~   
   $��5��6   $��1��6   $B <Введите целое натуральное число.��5      .��69   ��5�      �� 5�      $	 
HeadOffset�6   $ TwoWay=�6,   $ PropertyChanged=�6:    5�      5�      .��5�      5�      A� 5�   
   $��6��6   $��2��6#    A� Columns$ 2��5�      .+�5�      �� -   5         ��Name$ 	btnAccept��5�      $��Принять��5�      $��	10,5,10,5q�6'   5�      �� 5�      -   5        $ 	btnCancel��5�      $��Отменить��5�      $��	10,5,10,5q�6(   5�      5�      5�      5�      