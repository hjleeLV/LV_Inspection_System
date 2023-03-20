#pragma once
#include "StdAfx.h"

#include "Open_eVision_1_2.h"
using namespace Euresys::Open_eVision_1_2;

class CImPro_Find
{
public:
	CImPro_Find(void);
	~CImPro_Find(void);

public:
	EImageBW8 E_Cam_Img[4];							// Euresys ī�޶��̹���
	EPatternFinder m_Cam_Find[4];					// Euresys ���ε���
	vector<EFoundPattern> m_Cam_FoundPattern[4];	// Euresys ���ε���
	EImageBW8 EBW8Image1[4];
	double C_X[4];
	double C_Y[4];
	double C_A[4];

	bool J_EURESYS_Check();
	void J_Model_Learning(int Cam_num);
	void J_Model_Find(int Cam_num);
};

