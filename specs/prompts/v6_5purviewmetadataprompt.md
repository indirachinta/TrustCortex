Create IPurviewMetadataProvider abstraction.

Purpose:

Resolve governance metadata for retrieved documents.

Method:

Task<GovernanceMetadata?> GetMetadataAsync(
    string documentId,
    CancellationToken cancellationToken)

Create MockPurviewMetadataProvider.

Metadata source:

sample-data/purview-metadata.json

Register provider through dependency injection.