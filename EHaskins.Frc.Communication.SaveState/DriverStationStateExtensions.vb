Imports System.Runtime.CompilerServices
Imports EHaskins.Frc.Communication.DriverStation

Public Module DriverStationStateExtensions

    <Extension()>
    Public Function GetState(ByVal ds As DriverStation.DriverStation) As XElement
        Dim sticks As New List(Of SlimDXJoystick)
        For Each stick In ds.Joysticks
            If TypeOf (stick) Is SlimDXJoystick Then
                sticks.Add(stick)
            End If
        Next

        Dim udp As UdpTransmitter = ds.Connection
        Dim data = <DSState TeamNumber=<%= ds.TeamNumber %> PacketSize=<%= ds.PacketSize %> Enabled=<%= ds.IsEnabled %>>
                       <%= udp.GetState() %>
                       <Joysticks>
                           <%= From stick In sticks Select stick.GetStickState() %>
                       </Joysticks>
                   </DSState>
        Return Data
    End Function

    <Extension()>
    Public Sub LoadState(ByVal ds As DriverStation.DriverStation, ByVal data As XElement, ByVal manager As JoystickManager)
        ds.IsEnabled = False
        ds.TeamNumber = data.@TeamNumber
        ds.PacketSize = data.@PacketSize
        Dim udp As UdpTransmitter = ds.Connection
        udp.LoadState(data.<UDPTranceiver>(0))

        Dim stickData = data.<Joysticks>(0).<Joystick>

        For i = 0 To ds.Joysticks.Length - 1
            Dim stick As SlimDXJoystick = ds.Joysticks(i)
            stick.LoadState(stickData(i))
        Next
        ds.IsEnabled = data.@Enabled
    End Sub

    <Extension()>
    Public Function GetState(ByVal udp As UdpTransmitter) As XElement
        Return <UDPTranceiver Network=<%= udp.Network %> Host=<%= udp.Host %> ControlPort=<%= udp.TransmitPort %>
                   StatusPort=<%= udp.ReceivePort %>/>
    End Function

    <Extension()>
    Public Sub LoadState(ByVal udp As UdpTransmitter, ByVal data As XElement)
        udp.IsEnabled = False
        udp.ReceivePort = data.@StatusPort
        udp.TransmitPort = data.@ControlPort
        udp.Host = data.@Host
        udp.Network = data.@Network
    End Sub

    <Extension()>
    Public Function GetStickState(ByVal stick As SlimDXJoystick) As XElement
        Return <Joystick InstanceName=<%= stick.Name %>>
                   <Axes>
                       <%= From axis In stick.AxisData Select <Axis PhysicalAxis=<%= axis.PhysicalAxis %> Invert=<%= axis.Invert %>
                                                                  Deadband=<%= axis.Deadband %>/> %>
                   </Axes>
                   <Buttons>
                       <%= From button In stick.ButtonData Select <Button PhysicalButton=<%= button.PhysicalButton %> Invert=<%= button.Invert %>/> %>
                   </Buttons>
               </Joystick>
    End Function

    <Extension()>
    Public Sub LoadState(ByVal stick As SlimDXJoystick, ByVal data As XElement)
        stick.Name = data.@InstanceName
        Dim axes = data.<Axes>(0).<Axis>
        For i = 0 To stick.AxisData.Length - 1
            Dim axis = stick.AxisData(i)
            Dim axisData = axes(i)
            axis.PhysicalAxis = axisData.@PhysicalAxis
            axis.Deadband = axisData.@Deadband
            axis.Invert = axisData.@Invert
        Next

        Dim buttons = data.<Buttons>(0).<Button>
        For i = 0 To stick.ButtonData.Length - 1
            Dim button = stick.ButtonData(i)
            Dim buttonData = buttons(i)

            button.PhysicalButton = buttonData.@PhysicalButton
            button.Invert = buttonData.@Invert
        Next
    End Sub
End Module
