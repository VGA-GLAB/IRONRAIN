using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>QTE関係の情報を注入するためのクラス</summary>
public interface IProvidePlayerInformationInjectable
{
    public void InjectProProvidePlayerInformation(InGameManager.IProvidePlayerInformation providePlayerInformation);
}
