To change the encryption method, uncomment SI.Core - App.cs with the desired method.(only one per run is possible)
# Example:
```cs
public override void Initialize()
{
    /* Update View to change the encryption */
    //RegisterAppStart<DsaViewModel>();
    RegisterAppStart<DesViewModel>();
    //RegisterAppStart<RsaViewModel>();
}
```
