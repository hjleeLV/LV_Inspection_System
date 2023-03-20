#include "StdAfx.h"
#include "ImPro_Find.h"


CImPro_Find::CImPro_Find(void)
{
}


CImPro_Find::~CImPro_Find(void)
{
}

bool CImPro_Find::J_EURESYS_Check()
{
	return Easy::CheckLicense(LicenseFeatures::EasyFind);
}

void CImPro_Find::J_Model_Learning(int Cam_num)
{
	m_Cam_Find[Cam_num].SetMinScore(0.01);
	m_Cam_Find[Cam_num].SetInterpolate(true);
	m_Cam_Find[Cam_num].SetAngleTolerance(45.00f);
	m_Cam_Find[Cam_num].SetContrastMode(EFindContrastMode_Normal);
	m_Cam_Find[Cam_num].Learn(&E_Cam_Img[Cam_num]);
}

void CImPro_Find::J_Model_Find(int Cam_num)
{
// start timing
	const double tic_init = (double) getTickCount();

	if (m_Cam_Find[Cam_num].GetLearningDone())
	{	
		m_Cam_FoundPattern[Cam_num].clear();
		m_Cam_FoundPattern[Cam_num] = m_Cam_Find[Cam_num].Find(&EBW8Image1[Cam_num]);
		if (m_Cam_FoundPattern[Cam_num].size() > 0)
		{
			C_X[Cam_num] = m_Cam_FoundPattern[Cam_num][0].GetCenter().GetX();
			C_Y[Cam_num] = m_Cam_FoundPattern[Cam_num][0].GetCenter().GetY();
			C_A[Cam_num] = m_Cam_FoundPattern[Cam_num][0].GetAngle();
		}
		else
		{
			C_X[Cam_num] = 0;
			C_Y[Cam_num] = 0;
			C_A[Cam_num] = 0;
		}
	}
}
