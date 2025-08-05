using System;

// Token: 0x0200006E RID: 110
public interface ISpellCommand
{
	// Token: 0x060004B8 RID: 1208
	string GetSpellName();

	// Token: 0x060004B9 RID: 1209
	void TryCastSpell();

	// Token: 0x060004BA RID: 1210
	void ResetVoiceDetect();
}
