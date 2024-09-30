# Overview

This file contains the "physical" data objects for Unreal file archives, as they are represented on disk. They are not linked with objects from other archives, or usually even within their own archive.

# Name table

# Import/export tables

# Indices

Many of the physical data types refer to indices. An **index** in a UE3 package usually refers to either a 0-based index into the name table, or an index into the import/export tables. Notably, the import and export tables share an index space: negative values for an index mean that it's referring to an import, while positive values refer to an export.