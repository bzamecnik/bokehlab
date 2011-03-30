﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libpfm;
using mathHelper;

namespace spreading
{
    public class RectangleSpreadingFilter : AbstractSpreadingFilter
    {
        protected override void SpreadPSF(int x, int y, int radius, float weight, float[, ,] origImage, float[, ,] spreadingImage, float[, ,] normalizationImage, int tableWidth, int tableHeight, uint bands)
        {
            float psfSide = radius * 2 + 1; // side of a square PSF
            float areaInv = weight / (psfSide * psfSide);

            int top = MathHelper.Clamp<int>(y - radius, 0, tableHeight - 1);
            int bottom = MathHelper.Clamp<int>(y + radius + 1, 0, tableHeight - 1);
            int left = MathHelper.Clamp<int>(x - radius, 0, tableWidth - 1);
            int right = MathHelper.Clamp<int>(x + radius + 1, 0, tableWidth - 1);

            for (int band = 0; band < bands; band++)
            {
                float cornerValue = origImage[x, y, band] * areaInv;
                PutCorners(spreadingImage, top, bottom, left, right, band, cornerValue);
            }
            // Note: intensity for the normalization is 1.0
            PutCorners(normalizationImage, top, bottom, left, right, 0, areaInv);
        }

        private static void PutCorners(float[, ,] table, int top, int bottom, int left, int right, int band, float value)
        {
            table[left, top, band] += value;
            table[right, top, band] -= value;
            table[left, bottom, band] -= value;
            table[right, bottom, band] += value;
        }

        protected override void Integrate(PFMImage spreadingTable, PFMImage normalizationTable)
        {
            IntegrateHorizontally(spreadingTable, normalizationTable);
            IntegrateVertically(spreadingTable, normalizationTable);
        }
    }
}
