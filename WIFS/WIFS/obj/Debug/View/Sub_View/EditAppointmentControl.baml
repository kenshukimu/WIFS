   M S B A M L   `   `   `  ����  WIFS!    WIFS.EditAppointmentControl   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      ^ ZC1.WPF.Schedule.4, Version=4.0.20192.658, Culture=neutral, PublicKeyToken=2aa4ec5576d6c3ceU QC1.WPF.4, Version=4.0.20192.658, Culture=neutral, PublicKeyToken=2aa4ec5576d6c3cee aC1.WPF.DateTimeEditors.4, Version=4.0.20192.658, Culture=neutral, PublicKeyToken=2aa4ec5576d6c3ce<c1/http://schemas.componentone.com/winfx/2006/xaml    5      -   5          >�Name$
  root��5      #��� ��6   .��5      � -   5         �Name$ 
LayoutRoot��5       � 	Resources 5   
    "  BaseObjectItemPictTextTemplate � ColumnDefinitions �� CornerRadius��%~  &	        �� 5	      .R�5
      �  5      �� 5      $
��Auto �6+   �� 5      .+�5      �� $��imageBorder��6   
���   �$��1q�6D   $ 2��6X   $��15��6i   $��15��6u   $��Center=�6�   $��Center=�6�   $��3q�6�   ��6�   �� *Brush.Brush+5      �� 5      $��1��6    $��Center=�6F   ��60   �� *Text+6F   5      5      5       � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      �� 5      $
��Auto �6   .+�5   
   `� $��0��6   .�5      c�  c� Band$ 1��6    c� 	BandIndex$ 1��6   ��.  .@�5       �� -   5         ��Name$ PART_DialogSaveButton��5       ��.  $��Stretch=�6n   .��5!      �� 	 �� Orientation$	 
Horizontal=�6!   $��Center=�6:   .+�5"      �� $��Segoe UI Symbol)�6$   $��16(�6A   $��Center=�6O   $	����6j   �� 5#      $��3,0,0,0q�6$   $��Center=�65   �� 5$   
   .��6   ��5%      +  %C1.WPF.Schedule.SchedulerLocExtension  5&      -  'C1.WPF.Localization.C1LocalizeExtension

   Key$6
 0EditAppointment:PART_DialogSaveButton_AccessText��6      DefaultValue$ 저장��6S   .��5'      5(      5)      5*       � ToolTip 5+      �� 5,      ��5-        5.   "   $3
 -EditAppointment:PART_DialogSaveButton_Tooltip��62   $ Save and Close��6f   50      52      �� 5<      -   5        $��deleteButton��5<      ��.  $��Stretch=�6?   $-��'c1:C1Scheduler.DeleteAppointmentCommand��5?   :   ��5>      �� *:8ParentCollection.ParentStorage.ScheduleStorage.Scheduler+��5?      �� *Tag+.��5@      �� $	 
Horizontal=�6!   $��Center=�6:   .+�5A      �� $��Segoe UI Symbol)�6$   $��16(�6A   $��Center=�6O   $	����6j   �� 5B      $��Segoe UI Symbol)�6$   $��16(�6A   $��Center=�6O   $��삭제��6j   5C       5D      �� 5E      ��6$     $*
 $EditAppointment:deleteButton_ToolTip��$ Delete��5G      5H      5I      �� 5J   
   $��1��6   #��� ��6   $��5,5,5,0q�6j    �� Padding$ 5q�6{   $��	0,0,0,0.5q�6�   ._�5K      � $��
5, 0, 5, 5q�6    5L      �� 5M      �� 5N       5P      �� 5Q      $
��Auto �6'   �� 5R      �� 5S      $
��Auto �6'   �� 5T      $
��Auto �6'   .+�5W      �� $��Center=�6?   $��3,2,0,2q�6Z   $
��Left=�6k    �� Target 6   ��  �� ElementName$ subject����6�     $
 EditAppointment:subject��$ 일정제목���� 5X      -   5         ��Name$ subject��5X      $��0��6+   $��2q�68   $��1��6�   $��3��6�    �� 	MaxLength$	 255��6�   $��Center=�6�   $��10,2,0,2q�6   ��6D   �� *	Subject+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=�6   5[      5\      �� 5^   
   $��2��6   #��� ��6   $��5q�6j   $ 5q�6u   $��	0,0,0,0.5q�6�   ._�5_      � $��0,0,0,5q�6   .+�5`      � -   5        $��startEndPanel��5`       5a      �� 5b      �� 5c       5e      �� 5f      $
��Auto �6+   �� 5g      $
��Auto �6+   �� 5h      .+�5k      �� $��Center=�6   $��10,2,0,2q�67   $��0��6I   $
��Left=�6V    6q   �� $ startCalendar����5l        5m      $
 EditAppointment:startTime��6.   $ 시작시간:��6N   5o      -  'C1.WPF.DateTimeEditors.C1DateTimePicker  5p      -   5          Name$ startCalendar��5p   *   $��Center=�6A   $��10,2,0,2q�6\   $��1q�6n   ��.  $��1��6�      
TimeFormat$ 	ShortTime=�6     
DateFormat$
 Long=�6%     TimeIncrement$
 0:15h�67     FirstDayOfWeek 6�   ��  �� Path$W QParentCollection.ParentStorage.ScheduleStorage.Scheduler.CalendarHelper.WeekStart�   DateTime 6L  �� *Start+$ TwoWay=��� 5r      $��Center=�6   $��10,2,0,2q�67   $��1��6I   $
��Left=�6V    6q   �� $ endCalendar����5s        5t      $
 EditAppointment:endTime��6.   $ 종료시간:��6L   5v        5w      -   5        $ endCalendar��5w   *   $��Center=�6?   $��10,2,0,2q�6Z   $��1q�6l   $ 	ShortTime=�6  $
 Long=�6/  $
 0:15h�6A  $��1��6V  $��1��6f   6�   �� $W QParentCollection.ParentStorage.ScheduleStorage.Scheduler.CalendarHelper.WeekStart�6f  �� 5y      -	   5         ��Name$ 	chkAllDay��5y   J   $��2��6   $
��Left=�6/   $��Center=�6�   $��10,2,0,2q�6�    d� 	IsChecked 6�   �� *AllDayEvent+$ TwoWay=���5z        5{      $!
 EditAppointment:allDayEvent��6.   $ 종일 이벤트��6P   5}      5~      �� 5�      -
   5        $��recurrenceInfoPanel��5�      $y�	Collapsed=�68   $	 
Horizontal=�6O   .+�5�      �� ��5�        5�      $ 
 EditAppointment:recurrence��6.   $ Recurrence: ��6O   5�      �� 5�      -   5        $��recDescription��5�       ��66   �� *PatternDescription+$
 root��5�      5�      5�      �� 5�   
   $��4��6   $��5q�6   $ 5q�6)   ._�5�      �� .+�5�      �� -   5        $
 body��5�      $
��Auto=�6$    �� TextWrapping$
 Wrap=�6T    �� AcceptsReturn . $	��109��6�   ��6h   �� *Body+$ TwoWay=�6�   5�      5�      �� 5�   
   $��5��6   ._�5�      �� -   5        $ reminderSet��5�      $��Center=�6[   $��10,2,0,2q�6v   $y�Hidden=�5�   l    5�   ,   �� *ReminderSet+$ TwoWay=���5�        $!
 EditAppointment:reminderSet��$ 
Re_minder:��6l   5�      �� 5�   
   -   5        $ _hidid��5�      $y�Hidden=�6"   5�      5�      