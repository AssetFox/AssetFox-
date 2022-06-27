﻿using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.PamsData
{
    public static class ShortNamesForWorkTypes
    {
        internal static Dictionary<string, string> GetShortNamesForWorkTypes()
        {
            var shortNames = new Dictionary<string, string>
            {
                { "Crack Seal (HMA&COMP)", "Crack Seal (HMA&COMP)" },
                { "Spray Patch (HMA&COMP)", "Spray Patch (HMA&COMP)" },
                { "Skin Patch (HMA&COMP)", "Skin Patch (HMA&COMP)" },
                { "Manual Patch (HMA&COMP)", "Manual Patch (HMA&COMP)" },
                { "Manual Patch, Skin Patch (HMA&COMP)", "Manual Patch, Skin Patch (HMA&COMP)" },
                { "Mechanized Patch (HMA&COMP)", "Mechanized Patch (HMA&COMP)" },
                { "Mill, Manual Patch (HMA&COMP)", "Mill, Manual Patch (HMA&COMP)" },
                { "Mill, Mechanized Patch (HMA&COMP)", "Mill, Mechanized Patch (HMA&COMP)" },
                { "Mill, Mechanized Edge Patch (HMA&COMP)", "Mill, Mechanized Edge Patch (HMA&COMP)" },
                { "Base Repair, Manual Patch (HMA&COMP)", "Base Repair, Manual Patch (HMA&COMP)" },
                { "Base Repair, Mechanized Patch (HMA&COMP)", "Base Repair, Mechanized Patch (HMA&COMP)" },
                { "Seal Coat (HMA&COMP)", "Seal Coat (HMA&COMP)" },
                { "Level, Seal Coat (HMA&COMP)", "Level, Seal Coat (HMA&COMP)" },
                { "Widening, Seal Coat (HMA&COMP)", "Widening, Seal Coat (HMA&COMP)" },
                { "Scratch, Level, Seal Coat (HMA&COMP)", "Scratch, Level, Seal Coat (HMA&COMP)" },
                { "Microsurface/Thin Overlay (HMA&COMP)", "Microsurface/Thin Overlay (HMA&COMP)" },
                { "Level, Resurface (HMA&COMP)", "Level, Resurface (HMA&COMP)" },
                { "Mill, Conc. Patch, Level, Resurface (HMA&COMP)", "Mill, Conc. Patch, Level, Resurface (HMA&COMP)" },
                { "Level, Resurface, Base Repair (HMA&COMP)", "Level, Resurface, Base Repair (HMA&COMP)" },
                { "Mill, Level, Resurface (HMA&COMP)", "Mill, Level, Resurface (HMA&COMP)" },
                { "Mill, Level, Resurface, Base Repair (HMA&COMP)", "Mill, Level, Resurface, Base Repair (HMA&COMP)" },
                { "Construct Paved Shoulder (HMA&COMP)", "Construct Paved Shoulder (HMA&COMP)" },
                { "Reconstruction (HMA&COMP)", "Reconstruction (HMA&COMP)" },
                { "Joint Seal (JCP)", "Joint Seal (JCP)" },
                { "Crack Seal (JCP)", "Crack Seal (JCP)" },
                { "Spray Patch (JCP)", "Spray Patch (JCP)" },
                { "Bituminous Fill (Spall) w/Spray Patch (JCP)", "Bituminous Fill (Spall) w/Spray Patch (JCP)" },
                { "Partial Depth Bituminous Spall Repair (JCP)", "Partial Depth Bituminous Spall Repair (JCP)" },
                { "Base Repair w/ Mechanized Patch (JCP)", "Base Repair w/ Mechanized Patch (JCP)" },
                { "Stitching (JCP)", "Stitching (JCP)" },
                { "Dowel Bar Retrofit (JCP)", "Dowel Bar Retrofit (JCP)" },
                { "Slab Stabilization (JCP)", "Slab Stabilization (JCP)" },
                { "Spall Repair w/Transverse Joint Seal (JCP)", "Spall Repair w/Transverse Joint Seal (JCP)" },
                { "Diamond Grinding w/Transverse Joint Seal (JCP)", "Diamond Grinding w/Transverse Joint Seal (JCP)" },
                { "Concrete Patch (JCP)", "Concrete Patch (JCP)" },
                { "Slab Stabilization w/Diamond Grinding (JCP)", "Slab Stabilization w/Diamond Grinding (JCP)" },
                { "Concrete Patching w/Diamond Grinding (JCP)", "Concrete Patching w/Diamond Grinding (JCP)" },
                { "Slab Replacement (JCP)", "Slab Replacement (JCP)" },
                { "Microsurface/Thin Overlay (JCP)", "Microsurface/Thin Overlay (JCP)" },
                { "Bituminous Scratch Course w/Microsurface/Thin Overlay", "Bituminous Scratch Course w/Microsurface/Thin Overlay" },
                { "Partial Depth Bituminous Spall Repair w/Thin Overlay", "Partial Depth Bituminous Spall Repair w/Thin Overlay" },
                { "Base Repair w/Thin Overlay (JCP)", "Base Repair w/Thin Overlay (JCP)" },
                { "4” Bituminous Overlay (JCP)", "4” Bituminous Overlay (JCP)" },
                { "Bituminous Fill (Spall) w/4”Overlay (JCP)", "Bituminous Fill (Spall) w/4”Overlay (JCP)" },
                { "Stitching w/4” Overlay (JCP)", "Stitching w/4” Overlay (JCP)" },
                { "Dowel Bar Retrofit w/4” Overlay (JCP)", "Dowel Bar Retrofit w/4” Overlay (JCP)" },
                { "Concrete Patch w/4” Overlay (JCP)", "Concrete Patch w/4” Overlay (JCP)" },
                { "Bituminous Scratch Course w/4” Overlay (JCP)", "Bituminous Scratch Course w/4” Overlay (JCP)" },
                { "Slab Replacement w/4” Overlay (JCP)", "Slab Replacement w/4” Overlay (JCP)" },
                { "Reconstruction (JCP)", "Reconstruction (JCP)" },
                { "Microsurface/Thin Overlay & Manual Patch (HMA&COMP)", "Microsurface/Thin Overlay & Manual Patch (HMA&COMP)" },
                { "Diamond Grinding w/Transverse Joint Seal & Slab Stabilization (JCP)", "Diamond Grinding w/Transverse Joint Seal & Slab Stabilization (JCP)" },
                { "Concrete Patching w/Diamond Grinding & Joint Seal (JCP)", "Concrete Patching w/Diamond Grinding & Joint Seal (JCP)" },
                { "Concrete Patching w/Diamond Grinding & Slab Stabilization (JCP)", "Concrete Patching w/Diamond Grinding & Slab Stabilization (JCP)" },
                { "Bituminous Scratch Course w/Microsurface/Thin Overlay & Slab Stabilization (JCP)", "Bituminous Scratch Course w/Microsurface/Thin Overlay & Slab Stabilization (JCP)" },
                { "Bituminous Fill (Spall) w/4”Overlay & Slab Stabilization (JCP)", "Bituminous Fill (Spall) w/4”Overlay & Slab Stabilization (JCP)" },
                { "Stitching w/4” Overlay & Slab Stabilization (JCP)", "Stitching w/4” Overlay & Slab Stabilization (JCP)" },
                { "Dowel Bar Retrofit w/4” Overlay & Slab Stabilization (JCP)", "Dowel Bar Retrofit w/4” Overlay & Slab Stabilization (JCP)" },
                { "Concrete Patch w/4” Overlay & Slab Stabilization (JCP)", "Concrete Patch w/4” Overlay & Slab Stabilization (JCP)" },
                { "Bituminous Scratch Course w/4” Overlay & Slab Stabilization (JCP)", "Bituminous Scratch Course w/4” Overlay & Slab Stabilization (JCP)" },
                { "Slab Replacement w/4” Overlay & Slab Stabilization (JCP)", "Slab Replacement w/4” Overlay & Slab Stabilization (JCP)" },
                { "Bituminous Scratch Course w/Microsurface/Thin Overlay & Concrete Patch (JCP)", "Bituminous Scratch Course w/Microsurface/Thin Overlay & Concrete Patch (JCP)" },
                { "Bituminous Fill (Spall) w/4”Overlay & Concrete Patch (JCP)", "Bituminous Fill (Spall) w/4”Overlay & Concrete Patch (JCP)" },
                { "Stitching w/4” Overlay & Concrete Patch (JCP)", "Stitching w/4” Overlay & Concrete Patch (JCP)" },
                { "Dowel Bar Retrofit w/4” Overlay & Concrete Patch (JCP)", "Dowel Bar Retrofit w/4” Overlay & Concrete Patch (JCP)" },
                { "Concrete Patch w/4” Overlay & Concrete Patch (JCP)", "Concrete Patch w/4” Overlay & Concrete Patch (JCP)" },
                { "Bituminous Scratch Course w/4” Overlay & Concrete Patch (JCP)", "Bituminous Scratch Course w/4” Overlay & Concrete Patch (JCP)" },
                { "Slab Replacement w/4” Overlay & Concrete Patch (JCP)", "Slab Replacement w/4” Overlay & Concrete Patch (JCP)" },
                { "Bituminous Scratch Course w/4” Overlay & Bituminous Fill (Spall) w/Spray Patch (JCP)", "Bituminous Scratch Course w/4” Overlay & Bituminous Fill (Spall) w/Spray Patch (JCP)" },
                { "Bituminous Scratch Course w/4” Overlay & Stitching (JCP)", "Bituminous Scratch Course w/4” Overlay & Stitching (JCP)" },
                { "Bituminous Scratch Course w/4” Overlay & Dowel Bar Retrofit (JCP)", "Bituminous Scratch Course w/4” Overlay & Dowel Bar Retrofit (JCP)" },
                { "Bituminous Scratch Course w/4” Overlay & Slab Replacement (JCP)", "Bituminous Scratch Course w/4” Overlay & Slab Replacement (JCP)" },
                { "Bituminous Fill (Spall) w/4”Overlay & Stitching (JCP)", "Bituminous Fill (Spall) w/4”Overlay & Stitching (JCP)" },
                { "Bituminous Fill (Spall) w/4”Overlay & Dowel Bar Retrofit (JCP)", "Bituminous Fill (Spall) w/4”Overlay & Dowel Bar Retrofit (JCP)" },
                { "Bituminous Fill (Spall) w/4”Overlay & Slab Replacement (JCP)", "Bituminous Fill (Spall) w/4”Overlay & Slab Replacement (JCP)" },
                { "Stitching w/4” Overlay & Dowel Bar Retrofit (JCP)", "Stitching w/4” Overlay & Dowel Bar Retrofit (JCP)" },
                { "Slab Replacement w/4” Overlay & Stitching (JCP)", "Slab Replacement w/4” Overlay & Stitching (JCP)" },
                { "Slab Replacement w/4” Overlay & Dowel Bar Retrofit (JCP)", "Slab Replacement w/4” Overlay & Dowel Bar Retrofit (JCP)" },
            };
            return shortNames;
        }
    }
}
