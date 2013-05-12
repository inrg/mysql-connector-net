﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Security;
using System.Security.Permissions;
using System.Net;
using System.Web;
using System.Net.Mail;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.IO;


namespace MySql.Data.MySqlClient.Tests
{
  public class MediumTrustDomain :IDisposable
  {

    private AppDomain _domain;
    private string fullPathAssembly { get; set; }

    public MediumTrustDomain()
    {
      fullPathAssembly = @Path.GetFullPath(@".\..\..\..\..\Source\MySql.Data\bin\Release\MySql.Data.dll");
      registerAssembly();    
    }

    public AppDomain CreatePartialTrustAppDomain()
    {      
      PermissionSet permissions = new PermissionSet(PermissionState.None);
      permissions.AddPermission(new AspNetHostingPermission(AspNetHostingPermissionLevel.Medium));
      permissions.AddPermission(new DnsPermission(PermissionState.Unrestricted));
      permissions.AddPermission(new EnvironmentPermission(EnvironmentPermissionAccess.Read, "TEMP;TMP;USERNAME;OS;COMPUTERNAME"));
      permissions.AddPermission(new FileIOPermission(FileIOPermissionAccess.AllAccess, AppDomain.CurrentDomain.BaseDirectory));
      permissions.AddPermission(new IsolatedStorageFilePermission(PermissionState.None) { UsageAllowed = IsolatedStorageContainment.AssemblyIsolationByUser, UserQuota = Int64.MaxValue });
      permissions.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
      permissions.AddPermission(new SecurityPermission(SecurityPermissionFlag.ControlThread));
      permissions.AddPermission(new SecurityPermission(SecurityPermissionFlag.ControlPrincipal));
      permissions.AddPermission(new SecurityPermission(SecurityPermissionFlag.RemotingConfiguration));
      permissions.AddPermission(new SmtpPermission(SmtpAccess.Connect));
      permissions.AddPermission(new SqlClientPermission(PermissionState.Unrestricted));
      permissions.AddPermission(new TypeDescriptorPermission(PermissionState.Unrestricted));
      permissions.AddPermission(new WebPermission(PermissionState.Unrestricted));
      permissions.AddPermission(new ReflectionPermission(ReflectionPermissionFlag.RestrictedMemberAccess));
      permissions.AddPermission(new MySqlClientPermission(PermissionState.Unrestricted));
      permissions.AddPermission(new SocketPermission(PermissionState.Unrestricted));

      AppDomainSetup setup = new AppDomainSetup() { ApplicationBase = AppDomain.CurrentDomain.BaseDirectory };

      setup.PartialTrustVisibleAssemblies = new string[]
            {
                "System.Web, PublicKey=002400000480000094000000060200000024000052534131000400000100010007d1fa57c4aed9f0a32e84aa0faefd0de9e8fd6aec8f87fb03766c834c99921eb23be79ad9d5dcc1dd9ad236132102900b723cf980957fc4e177108fc607774f29e8320e92ea05ece4e821c0a5efe8f1645c4c0c93c1ab99285d622caa652c1dfad63d745d6f2de5f17e5eaf0fc4963d261c8a12436518206dc093344d5ad293",
                "MySql.Data, PublicKey=0024000004800000940000000602000000240000525341310004000001000100d973bda91f71752c78294126974a41a08643168271f65fc0fb3cd45f658da01fbca75ac74067d18e7afbf1467d7a519ce0248b13719717281bb4ddd4ecd71a580dfe0912dfc3690b1d24c7e1975bf7eed90e4ab14e10501eedf763bff8ac204f955c9c15c2cf4ebf6563d8320b6ea8d1ea3807623141f4b81ae30a6c886b3ee1"             
            };

      _domain = AppDomain.CreateDomain("MediumTrustSandbox", null, setup, permissions);
      return _domain;
    }

    public void Dispose()
    {
      if (_domain != null)
      {
        AppDomain.Unload(_domain);
        _domain = null;
        unregisterAssembly();
      }    
    }

    private void registerAssembly()
    {
      StringBuilder command = new StringBuilder();

      StringBuilder commandLineParams = new StringBuilder();
      commandLineParams.AppendFormat("/i \"{0}\"", fullPathAssembly);

      ProcessStartInfo processStartInfo = new ProcessStartInfo();

      command.AppendFormat("\"{0}\"", @Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86).ToString() + @"\Microsoft SDKs\Windows\v7.0A\Bin\NETFX 4.0 Tools\" + "gacutil.exe");
      processStartInfo.FileName = command.ToString();
      processStartInfo.Arguments = commandLineParams.ToString();
      processStartInfo.UseShellExecute = false;
      processStartInfo.Verb = "runas";
      var process = Process.Start(processStartInfo);
      process.WaitForExit();
    }


    private void unregisterAssembly()
    {
      StringBuilder command = new StringBuilder();

      StringBuilder commandLineParams = new StringBuilder();
      commandLineParams.AppendFormat("/u \"{0}\"", "mysql.data");

      ProcessStartInfo processStartInfo = new ProcessStartInfo();

      command.AppendFormat("\"{0}\"", @Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86).ToString() + @"\Microsoft SDKs\Windows\v7.0A\Bin\NETFX 4.0 Tools\" + "gacutil.exe");
      processStartInfo.FileName = command.ToString();
      processStartInfo.Arguments = commandLineParams.ToString();
      processStartInfo.UseShellExecute = false;
      processStartInfo.Verb = "runas";
      var process = Process.Start(processStartInfo);
      process.WaitForExit();
    }

  }   
}
